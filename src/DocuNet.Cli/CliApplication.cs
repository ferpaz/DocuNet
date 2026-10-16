namespace DocuNet.Cli;

public static class CliApplication
{
    public static int Run(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Contains("--help", StringComparer.Ordinal))
        {
            output.WriteLine("Usage: docunet [--output=path] [--templates=path] assembly.dll [pattern.dll ...] [pattern.xml ...]");
            output.WriteLine("DLL/EXE and XML patterns accept * and ?. XML defaults to companion files.");
            output.WriteLine("Default output: ./output. Default templates: Templates beside the executable.");
            output.WriteLine("Exit codes: 0 success/help, 2 arguments/inputs, 3 metadata/XML, 4 templates/output.");
            return 0;
        }
        try
        {
            string destination = "output";
            string templates = Path.Combine(AppContext.BaseDirectory, "Templates");
            var assemblies = new List<string>();
            var xmls = new List<string>();
            foreach (string arg in args)
            {
                if (arg.StartsWith("--output=", StringComparison.Ordinal))
                    destination = Value(arg);
                else if (arg.StartsWith("--templates=", StringComparison.Ordinal))
                    templates = Value(arg);
                else if (arg.StartsWith("--", StringComparison.Ordinal))
                    throw new ArgumentException($"Unknown argument: {arg}. Use --help.");
                else
                {
                    string extension = Path.GetExtension(arg);
                    if (extension.Equals(".xml", StringComparison.OrdinalIgnoreCase)) xmls.AddRange(Expand(arg));
                    else if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) || extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                        assemblies.AddRange(Expand(arg));
                    else throw new ArgumentException($"Unsupported input: {arg}.");
                }
            }
            if (assemblies.Count == 0) throw new ArgumentException("No assemblies specified.");
            if (xmls.Count == 0)
                xmls.AddRange(assemblies.Select(FindCompanionXml).OfType<string>());
            if (xmls.Count == 0) throw new ArgumentException("No XML documents found. Specify documentation XML explicitly.");
            foreach (string path in assemblies.Concat(xmls))
                if (!File.Exists(path)) throw new ArgumentException($"Input file not found: {path}.");
            var model = new MetadataDocumentationReader().Read(
                assemblies.Distinct(StringComparer.OrdinalIgnoreCase), xmls.Distinct(StringComparer.OrdinalIgnoreCase));
            int count = new SiteGenerator().Generate(model, templates, destination);
            output.WriteLine($"Generated {count} files, {model.Types.Count()} types and {model.Members.Count()} members.");
            return 0;
        }
        catch (ArgumentException ex) { error.WriteLine(ex.Message); return 2; }
        catch (DocumentationException ex) { error.WriteLine(ex.Message); return 3; }
        catch (RenderingException ex) { error.WriteLine(ex.Message); return 4; }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            error.WriteLine($"Input/output error: {ex.Message}");
            return 2;
        }
    }

    private static string? FindCompanionXml(string assembly)
    {
        string expected = Path.ChangeExtension(assembly, ".xml");
        if (File.Exists(expected)) return expected;
        string directory = Path.GetDirectoryName(assembly)!;
        var matches = Directory.EnumerateFiles(directory).Where(p =>
            Path.GetFileNameWithoutExtension(p).Equals(Path.GetFileNameWithoutExtension(assembly), StringComparison.OrdinalIgnoreCase)
            && Path.GetExtension(p).Equals(".xml", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (matches.Length > 1) throw new ArgumentException($"Ambiguous companion XML for {assembly}; specify it explicitly.");
        return matches.SingleOrDefault();
    }
    private static string Value(string arg)
    {
        string value = arg[(arg.IndexOf('=') + 1)..];
        return string.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"A path is required: {arg}.") : value;
    }

    private static IEnumerable<string> Expand(string pattern)
    {
        if (!pattern.Contains('*') && !pattern.Contains('?')) return [Path.GetFullPath(pattern)];
        string directory = Path.GetFullPath(Path.GetDirectoryName(pattern) is { Length: > 0 } dir ? dir : ".");
        if (!Directory.Exists(directory)) throw new ArgumentException($"Input directory not found: {directory}.");
        var files = Directory.GetFiles(directory, Path.GetFileName(pattern)).Order(StringComparer.Ordinal).ToArray();
        if (files.Length == 0) throw new ArgumentException($"No files match: {pattern}.");
        return files;
    }
}
