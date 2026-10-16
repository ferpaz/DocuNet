namespace DocuNet.Core;

public interface IReferencable
{
    string Id { get; }
    string Name { get; }
    string PrettyName { get; }
    string FullName { get; }
    CommentBlock Summary { get; }
    bool HasDocumentation { get; }
}

public sealed class CommentBlock(XElement? element = null)
{
    public XElement? Element { get; } = element;
    public IReadOnlyList<XNode> Children => Element?.Nodes().ToArray() ?? [];
    public bool IsEmpty => Children.Count == 0;
}

public abstract class DocumentationElement : IReferencable
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string PrettyName { get; init; }
    public required string FullName { get; init; }
    public CommentBlock Summary { get; set; } = new();
    public CommentBlock Remarks { get; set; } = new();
    public CommentBlock Value { get; set; } = new();
    public CommentBlock Example { get; set; } = new();
    public bool HasDocumentation => !Summary.IsEmpty || !Remarks.IsEmpty || !Value.IsEmpty;

    internal void ReadComments(XElement? xml)
    {
        Summary = new(xml?.Element("summary"));
        Remarks = new(xml?.Element("remarks"));
        Value = new(xml?.Element("value"));
        Example = new(xml?.Element("example"));
    }
}

public sealed class AssemblyDocumentation
{
    public required string Name { get; init; }
    public required Version Version { get; init; }
    public string ProductName { get; init; } = "";
    public string FileDescription { get; init; } = "";
    public required string Path { get; init; }
}

public sealed class NamespaceDocumentation : DocumentationElement
{
    public List<TypeDocumentation> Types { get; } = [];
    public IEnumerable<TypeDocumentation> Classes => Types.Where(t => !t.IsInterface);
    public IEnumerable<TypeDocumentation> Interfaces => Types.Where(t => t.IsInterface);
    public bool HasClasses => Classes.Any();
    public bool HasInterfaces => Interfaces.Any();
    public bool HasTypes => Types.Count > 0;
}

public sealed class TypeDocumentation : DocumentationElement
{
    public required NamespaceDocumentation Namespace { get; init; }
    public required string FileName { get; set; }
    public bool IsInterface { get; init; }
    public bool IsStatic { get; init; }
    public TypeReference? ParentType { get; init; }
    public List<TypeReference> Interfaces { get; } = [];
    public List<MethodDocumentation> Methods { get; } = [];
    public List<PropertyDocumentation> Properties { get; } = [];
    public List<FieldDocumentation> Fields { get; } = [];
    public List<EventDocumentation> Events { get; } = [];
}

public abstract class MemberDocumentation : DocumentationElement
{
    public required TypeDocumentation Type { get; init; }
    public required TypeReference ReturnType { get; init; }
    // XmlId is the declaring member's exact XML identifier, also for inherited members.
    public required string XmlId { get; init; }
    public bool IsInherited { get; init; }
    public bool IsStatic { get; init; }
}

public sealed class MethodDocumentation : MemberDocumentation
{
    public bool IsPublic => true;
    public bool IsExtension { get; init; }
    public List<ParameterDocumentation> Parameters { get; } = [];
    public CommentBlock Returns { get; set; } = new();
}

public sealed class PropertyDocumentation : MemberDocumentation
{
    public bool HasGet { get; init; }
    public bool HasSet { get; init; }
}

public sealed class FieldDocumentation : MemberDocumentation;
public sealed class EventDocumentation : MemberDocumentation;

public sealed class ParameterDocumentation
{
    public required string Name { get; init; }
    public required TypeReference Reference { get; init; }
    public CommentBlock Summary { get; init; } = new();
    public bool HasDocumentation => !Summary.IsEmpty;
}

public sealed record TypeReference(string XmlName, string PrettyName, string DefinitionName) : IReferencable
{
    public string Id => "T:" + DefinitionName;
    public string Name => DefinitionName.Split('.').Last();
    public string FullName => XmlName;
    public CommentBlock Summary { get; internal set; } = new();
    public bool HasDocumentation => !Summary.IsEmpty;
    internal IReadOnlyList<TypeReference> Arguments { get; init; } = [];
    internal bool IsNested { get; init; }
}

public sealed class DocumentationModel
{
    public List<AssemblyDocumentation> Assemblies { get; } = [];
    public List<NamespaceDocumentation> Namespaces { get; } = [];
    public IEnumerable<TypeDocumentation> Types => Namespaces.SelectMany(n => n.Types);
    public IEnumerable<MemberDocumentation> Members => Types.SelectMany(t =>
        t.Methods.Cast<MemberDocumentation>().Concat(t.Properties).Concat(t.Fields).Concat(t.Events));
}

public sealed class DocumentationException(string message, Exception? innerException = null)
    : Exception(message, innerException);
