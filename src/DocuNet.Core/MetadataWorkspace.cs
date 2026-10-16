namespace DocuNet.Core;

internal sealed class MetadataImage : IDisposable
{
    private readonly FileStream stream;
    private readonly PEReader pe;

    public MetadataImage(string path)
    {
        Path = System.IO.Path.GetFullPath(path);
        stream = File.OpenRead(Path);
        try
        {
            pe = new PEReader(stream);
            if (!pe.HasMetadata) throw new BadImageFormatException("The file has no managed metadata.");
            Reader = pe.GetMetadataReader();
            if (!Reader.IsAssembly) throw new BadImageFormatException("Expected an assembly, not a module.");
            Name = Reader.GetString(Reader.GetAssemblyDefinition().Name);
            IsFramework = Name == "mscorlib" || Reader.AssemblyReferences
                .Any(h => Reader.GetString(Reader.GetAssemblyReference(h).Name) == "mscorlib");
            foreach (var handle in Reader.TypeDefinitions)
                Types.Add(SignatureProvider.FullName(Reader, handle), handle);
        }
        catch { stream.Dispose(); throw; }
    }

    public string Path { get; }
    public string Name { get; }
    public bool IsFramework { get; }
    public MetadataReader Reader { get; }
    public Dictionary<string, TypeDefinitionHandle> Types { get; } = new(StringComparer.Ordinal);
    public void Dispose() { pe.Dispose(); stream.Dispose(); }
}

internal sealed class MetadataWorkspace : IDisposable
{
    private readonly List<MetadataImage> images = [];
    private readonly List<string> siblingPaths = [];
    private readonly List<string> platformPaths = [];
    private readonly List<string> frameworkPaths = [];

    public MetadataWorkspace(IEnumerable<string> inputs)
    {
        var paths = inputs.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        try
        {
            foreach (var directory in paths.Select(Path.GetDirectoryName).Distinct())
                siblingPaths.AddRange(Directory.EnumerateFiles(directory!, "*.dll"));
            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string platform)
                platformPaths.AddRange(platform.Split(Path.PathSeparator));
            string framework = Path.Combine(AppContext.BaseDirectory, "ReferenceAssemblies", "net48");
            if (Directory.Exists(framework))
                frameworkPaths.AddRange(Directory.EnumerateFiles(framework, "*.dll", SearchOption.AllDirectories)
                    .OrderBy(p => Path.GetFileName(p) == "mscorlib.dll" ? 0 : 1).ThenBy(p => p, StringComparer.Ordinal));
            foreach (var path in paths) Load(path);
            Inputs = images.ToArray();
        }
        catch { Dispose(); throw; }
    }

    public IReadOnlyList<MetadataImage> Inputs { get; }

    private MetadataImage Load(string path)
    {
        path = Path.GetFullPath(path);
        var existing = images.FirstOrDefault(i => string.Equals(i.Path, path, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) return existing;
        var image = new MetadataImage(path);
        images.Add(image);
        return image;
    }

    public (MetadataImage Image, TypeDefinitionHandle Handle) Resolve(string fullName, bool framework)
    {
        foreach (var image in Inputs.Concat(images.Where(i => i.IsFramework == framework)).Distinct())
            if (image.Types.TryGetValue(fullName, out var handle)) return (image, handle);
        // Reference assemblies are PE metadata data, not runtime dependencies to load into the CLR.
        foreach (string path in siblingPaths.Concat(framework ? frameworkPaths : platformPaths).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            MetadataImage image;
            try { image = Load(path); }
            catch (BadImageFormatException) { continue; }
            if (image.IsFramework == framework && image.Types.TryGetValue(fullName, out var handle))
                return (image, handle);
        }
        throw new DocumentationException($"Cannot resolve base type/interface '{fullName}'. Place its dependency DLL beside the input assembly; Framework inputs require the bundled net48 reference metadata.");
    }

    public void Dispose()
    {
        foreach (var image in images) image.Dispose();
    }
}
