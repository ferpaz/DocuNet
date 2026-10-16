namespace DocuNet.Tests;

public sealed class RenderingTests
{
    [Fact]
    public void GeneratesEscapedContentUniqueOverloadAnchorsAndValidLocalLinks()
    {
        using var workspace = new TestWorkspace();
        var model = TestWorkspace.ReadModel();
        var method = model.Types.Single(t => t.Name == "First").Methods.Single(m => m.Name == "Find" && m.Parameters.Count == 1);
        var parameter = method.Parameters[0];
        method.Parameters[0] = new DocuNet.Core.ParameterDocumentation { Name = "<parameter>", Reference = parameter.Reference, Summary = parameter.Summary };
        int count = new SiteGenerator().Generate(model, workspace.Templates, workspace.Output);
        Assert.Equal(18, count);
        string first = File.ReadAllText(Path.Combine(workspace.Output, "Fixture", "First.htm"));
        string second = File.ReadAllText(Path.Combine(workspace.Output, "Fixture", "Second.htm"));
        Assert.Contains("&lt;b&gt;literal&lt;/b&gt;", first);
        Assert.DoesNotContain("<b>literal</b>", first);
        Assert.Contains("&lt;parameter&gt;", first);
        Assert.DoesNotContain("<parameter>", first);
        Assert.Contains("FIRST method", first);
        Assert.Contains("SECOND method", second);
        var html = new HtmlParser().ParseDocument(first);
        Assert.Equal(2, html.QuerySelectorAll("h4").Count(h => h.TextContent.Contains("Find")));
        Assert.NotNull(html.GetElementById("Same")); // legacy alias remains where unambiguous
        Assert.Null(html.GetElementById("Find")); // an ambiguous alias must not hide an overload
        var audit = HtmlSiteInspector.Inspect(workspace.Output);
        Assert.Empty(audit.Failures);
        Assert.True(audit.Links > 50);
    }

    [Fact]
    public void ExternalTemplatePartialAndResourceEditsTakeEffectWithoutRebuilding()
    {
        using var workspace = new TestWorkspace();
        var model = TestWorkspace.ReadModel();
        var generator = new SiteGenerator();
        generator.Generate(model, workspace.Templates, workspace.Output);
        string footer = Path.Combine(workspace.Templates, "_common_footer.spark");
        File.AppendAllText(footer, "<p>TEMPLATE_EDIT_MARKER</p>");
        File.WriteAllText(Path.Combine(workspace.Templates, "extra.txt"), "RESOURCE_MARKER");
        generator.Generate(model, workspace.Templates, workspace.Output);
        Assert.Contains("TEMPLATE_EDIT_MARKER", File.ReadAllText(Path.Combine(workspace.Output, "index.htm")));
        Assert.Contains("TEMPLATE_EDIT_MARKER", File.ReadAllText(Path.Combine(workspace.Output, "Fixture", "First.htm")));
        Assert.Equal("RESOURCE_MARKER", File.ReadAllText(Path.Combine(workspace.Output, "extra.txt")));
        File.Delete(Path.Combine(workspace.Templates, "extra.txt"));
        generator.Generate(model, workspace.Templates, workspace.Output);
        Assert.False(File.Exists(Path.Combine(workspace.Output, "extra.txt")));
    }

    [Fact]
    public void RegenerationIsDeterministicAndPreservesUnmanagedVersions()
    {
        using var workspace = new TestWorkspace();
        string otherVersion = Path.Combine(workspace.Output, "versions", "previous", "index.htm");
        Directory.CreateDirectory(Path.GetDirectoryName(otherVersion)!);
        File.WriteAllText(otherVersion, "KEEP");
        var model = TestWorkspace.ReadModel();
        var generator = new SiteGenerator();
        generator.Generate(model, workspace.Templates, workspace.Output);
        var first = Hashes(workspace.Output);
        generator.Generate(model, workspace.Templates, workspace.Output);
        Assert.Equal(first, Hashes(workspace.Output));
        Assert.Equal("KEEP", File.ReadAllText(otherVersion));
    }

    [Fact]
    public void FailingTemplateDoesNotReplaceExistingOutput()
    {
        using var workspace = new TestWorkspace();
        var generator = new SiteGenerator();
        var model = TestWorkspace.ReadModel();
        generator.Generate(model, workspace.Templates, workspace.Output);
        var before = Hashes(workspace.Output);
        File.WriteAllText(Path.Combine(workspace.Templates, "broken.htm.spark"), "${MissingIdentifier}");
        Assert.Throws<RenderingException>(() => generator.Generate(model, workspace.Templates, workspace.Output));
        Assert.Equal(before, Hashes(workspace.Output));
    }

    [Fact]
    public void RefusesUnmanagedCollisionAndEditedManagedFiles()
    {
        using var workspace = new TestWorkspace();
        Directory.CreateDirectory(workspace.Output);
        string index = Path.Combine(workspace.Output, "index.htm");
        File.WriteAllText(index, "UNMANAGED");
        var generator = new SiteGenerator();
        var model = TestWorkspace.ReadModel();
        Assert.Throws<RenderingException>(() => generator.Generate(model, workspace.Templates, workspace.Output));
        Assert.Equal("UNMANAGED", File.ReadAllText(index));
        File.Delete(index);
        generator.Generate(model, workspace.Templates, workspace.Output);
        File.WriteAllText(index, "MANUAL_EDIT");
        Assert.Throws<RenderingException>(() => generator.Generate(model, workspace.Templates, workspace.Output));
        Assert.Equal("MANUAL_EDIT", File.ReadAllText(index));
    }

    [Fact]
    public void RejectsTraversalInManagedManifestBeforeChangingAnyOutput()
    {
        using var workspace = new TestWorkspace();
        Directory.CreateDirectory(workspace.Output);
        string outside = Path.Combine(workspace.Root, "outside.txt");
        File.WriteAllText(outside, "KEEP");
        string hash = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(outside)));
        File.WriteAllText(Path.Combine(workspace.Output, ".docunet-manifest.json"),
            JsonSerializer.Serialize(new { Files = new Dictionary<string, string> { ["../outside.txt"] = hash } }));
        Assert.Throws<RenderingException>(() => new SiteGenerator().Generate(TestWorkspace.ReadModel(), workspace.Templates, workspace.Output));
        Assert.Equal("KEEP", File.ReadAllText(outside));
    }

    internal static string[] Hashes(string root) => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        .Order(StringComparer.Ordinal).Select(p => Path.GetRelativePath(root, p) + " " +
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p)))).ToArray();
}
