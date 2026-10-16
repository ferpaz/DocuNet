using Spark;

namespace DocuNet.Rendering;

public abstract class TemplateView : AbstractSparkView
{
    public ViewData ViewData { get; set; } = null!;
    public IReadOnlyList<AssemblyDocumentation> Assemblies => ViewData.Assemblies;
    public IReadOnlyList<NamespaceDocumentation> Namespaces => ViewData.Namespaces;
    public IReadOnlyList<TypeDocumentation> Types => ViewData.Types;
    public NamespaceDocumentation Namespace => ViewData.Namespace!;
    public TypeDocumentation Type => ViewData.Type!;
    public string RelativeOutputPath { get; set; } = "";
    public string NamespaceUrlFormat { get; private set; } = "~/{namespace}/index.htm";
    public string TypeUrlFormat { get; private set; } = "~/{type.namespace}/{type}.htm";
    public string MethodUrlFormat { get; private set; } = "~/{type.namespace}/{type}.htm#{method}";
    public string PropertyUrlFormat { get; private set; } = "~/{type.namespace}/{type}.htm#{property}";
    public string FieldUrlFormat { get; private set; } = "~/{type.namespace}/{type}.htm#{field}";
    public string EventUrlFormat { get; private set; } = "~/{type.namespace}/{type}.htm#{event}";

    // Legacy Spark templates already use h() and Format() for trusted markup.
    public override void OutputValue(object value, bool automaticEncoding) => Output.Write(value);
    public string h(string? text) => WebUtility.HtmlEncode(text ?? "");
    public string WriteVersion(AssemblyDocumentation assembly) => assembly.Version.ToString();
    public string WriteProductName(AssemblyDocumentation assembly) => h(assembly.ProductName);
    public string WriteFileDescription(AssemblyDocumentation assembly) => h(assembly.FileDescription);
    public string WriteAssemblyTitle(AssemblyDocumentation assembly) => h(
        string.IsNullOrEmpty(assembly.FileDescription) || assembly.ProductName == assembly.FileDescription ? assembly.ProductName
        : string.IsNullOrEmpty(assembly.ProductName) ? assembly.FileDescription
        : assembly.FileDescription + " (" + assembly.ProductName + ")");

    public string SiteResource(string path) => !path.StartsWith("~/", StringComparison.Ordinal) ? path
        : string.Concat(Enumerable.Repeat("../", RelativeOutputPath.Count(c => c == '/'))) + path[2..];

    public string Anchor(IReferencable member) =>
        "member-" + Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(member.Id)));

    public string LegacyAnchor(MemberDocumentation member) =>
        Type.Methods.Cast<MemberDocumentation>().Concat(Type.Properties).Concat(Type.Fields).Concat(Type.Events)
            .Count(m => m.Name == member.Name) == 1 ? h(member.Name) : "";

    public string Format(IReferencable reference, params Expression<Func<object, string>>[] attributes)
    {
        IReferencable? target = reference switch
        {
            TypeReference typeRef => ViewData.Model.Types.FirstOrDefault(t => t.Id == typeRef.Id),
            _ => reference
        };
        if (target is null) return "<span title=\"" + h(reference.FullName) + "\">" + h(reference.PrettyName) + "</span>";
        string path;
        if (target is NamespaceDocumentation ns)
            path = NamespaceUrlFormat.Replace("{namespace}", Uri.EscapeDataString(ns.Name), StringComparison.Ordinal);
        else if (target is TypeDocumentation type)
            path = TypePath(type, TypeUrlFormat);
        else if (target is MemberDocumentation member)
        {
            string pattern = member switch
            {
                MethodDocumentation => MethodUrlFormat, PropertyDocumentation => PropertyUrlFormat,
                FieldDocumentation => FieldUrlFormat, _ => EventUrlFormat
            };
            path = TypePath(member.Type, pattern).Replace("{method}", Anchor(member), StringComparison.Ordinal)
                .Replace("{property}", Anchor(member), StringComparison.Ordinal)
                .Replace("{field}", Anchor(member), StringComparison.Ordinal).Replace("{event}", Anchor(member), StringComparison.Ordinal);
        }
        else return h(reference.PrettyName);
        string extra = "";
        foreach (var attribute in attributes)
        {
            string name = attribute.Parameters.Single().Name ?? "";
            if (!Regex.IsMatch(name, "^[A-Za-z][A-Za-z0-9_-]*$")) throw new RenderingException("Invalid link attribute name.");
            string value = attribute.Compile()(target);
            extra += " " + name + "=\"" + h(value) + "\"";
        }
        return "<a href=\"" + h(SiteResource(path)) + "\"" + extra + ">" + h(target.PrettyName) + "</a>";
    }

    private static string TypePath(TypeDocumentation type, string pattern) => pattern
        .Replace("{type.namespace}", Uri.EscapeDataString(type.Namespace.Name), StringComparison.Ordinal)
        .Replace("{type}", Uri.EscapeDataString(Path.GetFileNameWithoutExtension(type.FileName)), StringComparison.Ordinal);

    public string Format(CommentBlock comment) => CommentFormatter.Format(comment, this);
    public string WriteInterfaces(IEnumerable<TypeReference> interfaces) => string.Join(", ", interfaces.Select(i => Format(i)));
    public string OutputMethodParams(MethodDocumentation method) => string.Join(", ", method.Parameters.Select((p, i) =>
        (method.IsExtension && i == 0 ? "this " : "") + Format(p.Reference) + " " + h(p.Name)));
    public void SetNamespaceUrlFormat(string value) => NamespaceUrlFormat = value;
    public void SetTypeUrlFormat(string value) => TypeUrlFormat = value;
    public void SetMethodUrlFormat(string value) => MethodUrlFormat = value;
    public void SetPropertyUrlFormat(string value) => PropertyUrlFormat = value;
    public void SetFieldUrlFormat(string value) => FieldUrlFormat = value;
    public void SetEventUrlFormat(string value) => EventUrlFormat = value;
}

public static class TemplateHelperExtensions
{
    public static bool IsDocumented(this IReferencable element) => !element.Summary.IsEmpty;
}
