namespace DocuNet.Tests;

internal sealed record SiteInspection(int HtmlFiles, int Links, int MemberBlocks, IReadOnlyList<string> Failures);

internal static class HtmlSiteInspector
{
    public static SiteInspection Inspect(string root)
    {
        root = Path.GetFullPath(root);
        var knownFiles = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(Path.GetFullPath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var pages = knownFiles.Where(p => p.EndsWith(".htm", StringComparison.OrdinalIgnoreCase)).ToArray();
        var pageIds = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        var fragmentLinks = new Dictionary<(string Path, string Fragment), string>();
        var failures = new List<string>();
        var parser = new HtmlParser();
        int links = 0;
        int blocks = 0;
        foreach (string page in pages)
        {
            using var document = parser.ParseDocument(File.ReadAllText(page));
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var element in document.QuerySelectorAll("[id]"))
                if (!ids.Add(element.Id!)) failures.Add($"{Path.GetRelativePath(root, page)}: duplicate id {element.Id}");
            pageIds.Add(page, ids);
            blocks += document.QuerySelectorAll("div.method").Length;
            string pageUrl = "https://docunet.invalid/" + string.Join('/', Path.GetRelativePath(root, page)
                .Replace(Path.DirectorySeparatorChar, '/').Split('/').Select(Uri.EscapeDataString));
            var baseUri = new Uri(pageUrl);
            foreach (var element in document.QuerySelectorAll("[href], [src]"))
            {
                string value = element.GetAttribute("href") ?? element.GetAttribute("src") ?? "";
                if (string.IsNullOrEmpty(value) || Uri.TryCreate(value, UriKind.Absolute, out var absolute)
                    && absolute.Scheme is not "file") continue;
                links++;
                var url = new Uri(baseUri, value);
                string target = Path.GetFullPath(Path.Combine(root, Uri.UnescapeDataString(url.AbsolutePath)
                    .TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
                if (!knownFiles.Contains(target))
                    failures.Add($"{Path.GetRelativePath(root, page)}: missing/outside {value}");
                else if (url.Fragment.Length > 1)
                    fragmentLinks.TryAdd((target, Uri.UnescapeDataString(url.Fragment[1..])),
                        $"{Path.GetRelativePath(root, page)}: missing fragment {value}");
            }
        }
        foreach (var link in fragmentLinks)
            if (pageIds.TryGetValue(link.Key.Path, out var ids) && !ids.Contains(link.Key.Fragment))
                failures.Add(link.Value);
        return new(pages.Length, links, blocks, failures);
    }
}
