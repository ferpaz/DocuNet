namespace DocuNet.Rendering;

/// <summary>Stages a complete render and replaces only unchanged, previously managed files.</summary>
public sealed class SiteGenerator
{
    private const string ManifestName = ".docunet-manifest.json";
    private sealed record Manifest(Dictionary<string, string> Files);

    public int Generate(DocumentationModel model, string templateDirectory, string destination)
    {
        try { return GenerateCore(model, templateDirectory, destination); }
        catch (RenderingException) { throw; }
        catch (Exception ex) { throw new RenderingException($"Cannot prepare output: {ex.Message}", ex); }
    }

    private static int GenerateCore(DocumentationModel model, string templateDirectory, string destination)
    {
        string templates = Path.GetFullPath(templateDirectory);
        string output = Path.TrimEndingDirectorySeparator(Path.GetFullPath(destination));
        string? parent = Path.GetDirectoryName(output);
        if (parent is null || output == Path.GetPathRoot(output))
            throw new RenderingException("The output must not be a filesystem root.");
        if (!Directory.Exists(templates)) throw new RenderingException($"Template directory not found: {templates}.");
        if (IsWithin(output, templates) || IsWithin(templates, output))
            throw new RenderingException("Template and output directories must not contain each other.");
        AssertNoLinks(templates);
        AssertNoLinks(output);
        Directory.CreateDirectory(parent);
        string stage = Path.Combine(parent, ".docunet-stage-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(stage);
            var generated = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var renderer = new SparkRenderer(templates);
            var sourceFiles = Walk(templates).Order(StringComparer.Ordinal).ToArray();
            foreach (string source in sourceFiles.Where(p => p.EndsWith(".spark", StringComparison.Ordinal)))
            {
                string relative = Path.GetRelativePath(templates, source).Replace('\\', '/');
                foreach (var target in TemplateExpander.Expand(relative, model))
                {
                    string text = renderer.Render(relative, target.OutputPath, target.Data);
                    WriteStaged(stage, target.OutputPath, Encoding.UTF8.GetBytes(text), generated);
                }
            }
            if (generated.Count == 0) throw new RenderingException("No page templates found.");
            foreach (string source in sourceFiles.Where(p => !p.EndsWith(".spark", StringComparison.Ordinal)))
            {
                if (HasHiddenComponent(source, templates)) continue;
                WriteStaged(stage, Path.GetRelativePath(templates, source).Replace('\\', '/'), File.ReadAllBytes(source), generated);
            }
            Commit(stage, output, generated);
            return generated.Count;
        }
        catch (RenderingException) { throw; }
        catch (Exception ex) { throw new RenderingException($"Cannot generate the site: {ex.Message}", ex); }
        finally
        {
            // Only the fresh stage created by this invocation may be removed recursively.
            if (Path.GetDirectoryName(stage) == parent
                && Path.GetFileName(stage).StartsWith(".docunet-stage-", StringComparison.Ordinal)
                && Directory.Exists(stage)) Directory.Delete(stage, true);
        }
    }

    private static bool IsWithin(string path, string root) =>
        path.Equals(root, StringComparison.OrdinalIgnoreCase)
        || path.StartsWith(Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    private static IEnumerable<string> Walk(string root)
    {
        foreach (string entry in Directory.EnumerateFileSystemEntries(root))
        {
            if ((File.GetAttributes(entry) & FileAttributes.ReparsePoint) != 0)
                throw new RenderingException($"Linked template paths are not supported: {entry}.");
            if (Directory.Exists(entry))
                foreach (string file in Walk(entry)) yield return file;
            else yield return entry;
        }
    }

    private static bool HasHiddenComponent(string path, string root)
    {
        while (path != root)
        {
            if ((File.GetAttributes(path) & FileAttributes.Hidden) != 0) return true;
            path = Path.GetDirectoryName(path)!;
        }
        return false;
    }

    private static void WriteStaged(string stage, string relative, byte[] contents, Dictionary<string, string> generated)
    {
        string path = SafePath(stage, relative);
        if (relative.Equals(ManifestName, StringComparison.OrdinalIgnoreCase) || !generated.TryAdd(relative, Hash(contents)))
            throw new RenderingException($"Duplicate or reserved output path: {relative}.");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, contents);
    }

    private static string SafePath(string root, string relative)
    {
        if (Path.IsPathRooted(relative) || relative.Contains('\\')
            || relative.Split('/').Any(p => p is "" or "." or ".." || p.IndexOfAny([':', '*', '?', '"', '<', '>', '|']) >= 0
                || p.EndsWith(' ') || p.EndsWith('.')))
            throw new RenderingException($"Invalid relative output path: {relative}.");
        string path = Path.GetFullPath(Path.Combine(root, relative));
        if (!IsWithin(path, root) || path == root) throw new RenderingException($"Output path escapes destination: {relative}.");
        AssertNoLinks(path);
        return path;
    }

    private static void AssertNoLinks(string path)
    {
        for (string? current = path; current is not null; current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current))
                && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new RenderingException($"Linked paths are not supported: {current}.");
    }

    private static string Hash(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));

    private static void Commit(string stage, string output, Dictionary<string, string> generated)
    {
        string manifestPath = Path.Combine(output, ManifestName);
        AssertNoLinks(manifestPath);
        Manifest? previous = File.Exists(manifestPath)
            ? JsonSerializer.Deserialize<Manifest>(File.ReadAllText(manifestPath)) : null;
        var old = previous?.Files ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in old)
        {
            string path = SafePath(output, file.Key);
            if (File.Exists(path) && Hash(File.ReadAllBytes(path)) != file.Value)
                throw new RenderingException($"Managed file was edited; refusing to overwrite it: {file.Key}.");
        }
        foreach (string relative in generated.Keys)
        {
            string path = SafePath(output, relative);
            if (Directory.Exists(path) || File.Exists(path) && !old.ContainsKey(relative))
                throw new RenderingException($"Output collides with an unmanaged path: {relative}.");
        }
        Directory.CreateDirectory(output);
        foreach (var relative in generated.Keys)
        {
            string destination = SafePath(output, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(SafePath(stage, relative), destination, true);
        }
        foreach (string stale in old.Keys.Except(generated.Keys, StringComparer.OrdinalIgnoreCase))
            File.Delete(SafePath(output, stale));
        string manifestJson = JsonSerializer.Serialize(new Manifest(generated), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(manifestPath, manifestJson, new UTF8Encoding(false));
    }
}
