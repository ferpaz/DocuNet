namespace DocuNet.Rendering;

public sealed record TemplateOutput(string TemplatePath, string OutputPath, ViewData Data);

public static class TemplateExpander
{
    public static IEnumerable<TemplateOutput> Expand(string relativeTemplate, DocumentationModel model)
    {
        string normalized = relativeTemplate.Replace('\\', '/');
        if (!normalized.EndsWith(".spark", StringComparison.Ordinal)) yield break;
        if (Path.GetFileName(normalized).StartsWith('_')) yield break;
        string[] parts = normalized[..^6].Split('/');
        if (parts.Contains("layouts", StringComparer.Ordinal)) yield break;
        IEnumerable<(List<string> Path, NamespaceDocumentation? Namespace, TypeDocumentation? Type)> states =
            [(new List<string>(), null, null)];
        for (int index = 0; index < parts.Length; index++)
        {
            string part = parts[index];
            bool filename = index == parts.Length - 1;
            var next = new List<(List<string>, NamespaceDocumentation?, TypeDocumentation?)>();
            foreach (var state in states)
            {
                if (part == "!namespace" || filename && part.StartsWith("!namespace", StringComparison.Ordinal))
                {
                    foreach (var ns in state.Namespace is null ? model.Namespaces : [state.Namespace])
                        next.Add((state.Path.Append(part.Replace("!namespace", ns.Name, StringComparison.Ordinal)).ToList(), ns, state.Type));
                }
                else if (part == "!type" || filename && part.StartsWith("!type", StringComparison.Ordinal))
                {
                    foreach (var type in state.Type is not null ? [state.Type]
                        : state.Namespace?.Types ?? model.Types.ToList())
                    {
                        string name = Path.GetFileNameWithoutExtension(type.FileName);
                        if (!filename || state.Namespace is null) name = type.Namespace.Name + "." + name;
                        next.Add((state.Path.Append(part.Replace("!type", name, StringComparison.Ordinal)).ToList(), type.Namespace, type));
                    }
                }
                else next.Add((state.Path.Append(part).ToList(), state.Namespace, state.Type));
            }
            states = next;
        }
        foreach (var state in states)
            yield return new(relativeTemplate, string.Join('/', state.Path),
                new ViewData { Model = model, Namespace = state.Namespace, Type = state.Type });
    }
}
