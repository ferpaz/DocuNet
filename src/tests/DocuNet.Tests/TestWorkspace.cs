namespace DocuNet.Tests;

internal sealed class TestWorkspace : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "DocuNet-Tests", Guid.NewGuid().ToString("N"));
    public string Output => Path.Combine(Root, "site");
    public string Templates => Path.Combine(Root, "Templates");
    public static string AssemblyPath => Path.Combine(AppContext.BaseDirectory, "Fixtures", "Model.dll");
    public static string XmlPath => Path.Combine(AppContext.BaseDirectory, "Fixtures", "Model.xml");
    public static DocumentationModel ReadModel() => new MetadataDocumentationReader().Read([AssemblyPath], [XmlPath]);

    public TestWorkspace()
    {
        Directory.CreateDirectory(Root);
        Copy(Path.Combine(AppContext.BaseDirectory, "Templates"), Templates);
    }

    private static void Copy(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.GetFiles(source)) File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
        foreach (var directory in Directory.GetDirectories(source)) Copy(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }

    public void Dispose()
    {
        string allowed = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "DocuNet-Tests")) + Path.DirectorySeparatorChar;
        if (Path.GetFullPath(Root).StartsWith(allowed, StringComparison.Ordinal) && Directory.Exists(Root))
            Directory.Delete(Root, true);
    }
}
