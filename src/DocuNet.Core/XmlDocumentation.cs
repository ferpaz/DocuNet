namespace DocuNet.Core;

public sealed class XmlDocumentation
{
    private readonly Dictionary<string, XElement> members = new(StringComparer.Ordinal);

    public static XmlDocumentation Read(IEnumerable<string> paths)
    {
        var result = new XmlDocumentation();
        foreach (string path in paths)
        {
            try
            {
                using var reader = XmlReader.Create(path, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                });
                var document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
                if (document.Root?.Name != "doc" || document.Root.Element("members") is not { } container)
                    throw new DocumentationException($"Invalid documentation XML: {path} (expected doc/members).");
                foreach (var member in container.Elements("member"))
                {
                    string id = (string?)member.Attribute("name")
                        ?? throw new DocumentationException($"XML member without name: {path}.");
                    if (!result.members.TryAdd(id, member))
                        throw new DocumentationException($"Duplicate XML member: {id} in {path}.");
                }
            }
            catch (XmlException ex)
            {
                throw new DocumentationException($"Invalid documentation XML: {path}: {ex.Message}", ex);
            }
        }
        return result;
    }

    public XElement? Find(string id) => members.GetValueOrDefault(id);
}
