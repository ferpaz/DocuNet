namespace DocuNet.Rendering;

internal static class CommentFormatter
{
    public static string Format(CommentBlock block, TemplateView view) =>
        FormatNodes(block.Children, view, block.Element?.Name.LocalName == "example");

    private static string FormatNodes(IEnumerable<XNode> nodes, TemplateView view, bool preserve = false) =>
        string.Join(" ", nodes.Select(n => FormatNode(n, view, preserve)).Where(s => s.Length > 0)).Replace(" .", ".", StringComparison.Ordinal);

    private static string FormatNode(XNode node, TemplateView view, bool preserve)
    {
        if (node is XText text) return view.h(preserve ? text.Value : Regex.Replace(text.Value, @"\s+", " ").Trim());
        if (node is not XElement element) return "";
        string Children() => FormatNodes(element.Nodes(), view, preserve);
        return element.Name.LocalName switch
        {
            "para" => "<p>" + Children() + "</p>",
            "c" or "code" => "<code>" + view.h(element.Value.Trim()) + "</code>",
            "paramref" => "<var>" + view.h((string?)element.Attribute("name")) + "</var>",
            "see" => Reference((string?)element.Attribute("cref"), view),
            "list" => List(element, view),
            _ => ""
        };
    }

    private static string Reference(string? id, TemplateView view)
    {
        if (string.IsNullOrEmpty(id)) return "";
        IReferencable? target = view.ViewData.Model.Types.Cast<IReferencable>()
            .Concat(view.ViewData.Model.Namespaces).Concat(view.ViewData.Model.Members)
            .FirstOrDefault(m => m.Id == id);
        if (target is not null) return view.Format(target);
        string name = id.Length > 2 ? id[2..] : id;
        if (id.StartsWith("T:", StringComparison.Ordinal))
        {
            string pretty = name.Split('.').Last();
            return view.Format(new TypeReference(name, pretty, name));
        }
        string shortName = name.Split('(')[0].Split('.').Last();
        return "<span title=\"" + view.h(name) + "\">" + view.h(shortName) + "</span>";
    }

    private static string List(XElement element, TemplateView view)
    {
        string kind = (string?)element.Attribute("type") ?? "definition";
        var rows = element.Elements("item").Select(item =>
        {
            string term = FormatNodes(item.Element("term")?.Nodes() ?? [], view);
            string definition = FormatNodes(item.Element("description")?.Nodes() ?? [], view);
            return kind switch
            {
                "bullet" or "number" => "<li>" + term + definition + "</li>",
                "table" => "<tr><td>" + term + "</td><td>" + definition + "</td></tr>",
                _ => "<dt>" + term + "</dt><dd>" + definition + "</dd>"
            };
        });
        string tag = kind switch { "bullet" => "ul", "number" => "ol", "table" => "table", _ => "dl" };
        return "<" + tag + ">" + string.Concat(rows) + "</" + tag + ">";
    }
}
