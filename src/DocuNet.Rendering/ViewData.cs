namespace DocuNet.Rendering;

public sealed class ViewData
{
    public required DocumentationModel Model { get; init; }
    public IReadOnlyList<AssemblyDocumentation> Assemblies => Model.Assemblies;
    public IReadOnlyList<NamespaceDocumentation> Namespaces => Model.Namespaces;
    public IReadOnlyList<TypeDocumentation> Types => Model.Types.ToArray();
    public NamespaceDocumentation? Namespace { get; init; }
    public TypeDocumentation? Type { get; init; }
}

public sealed class RenderingException(string message, Exception? innerException = null) : Exception(message, innerException);
