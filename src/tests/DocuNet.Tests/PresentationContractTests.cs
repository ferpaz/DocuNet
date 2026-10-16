using System.Xml.Linq;

namespace DocuNet.Tests;

public sealed class PresentationContractTests
{
    [Fact]
    public void FormatsSupportedXmlAndResolvesReferencesWithoutEmittingUnknownMarkup()
    {
        using var workspace = new TestWorkspace();
        var model = TestWorkspace.ReadModel();
        model.Types.Single(t => t.FullName == "Fixture.First").Summary = new CommentBlock(XElement.Parse("""
            <summary>
              <para>Paragraph &amp; text <paramref name="input"/>.</para>
              <c>&lt;script&gt;literal&lt;/script&gt;</c><code>a &lt; b</code>
              <see cref="T:Fixture.Second"/><see cref="M:Fixture.Second.Same"/>
              <see cref="T:System.String"/><see cref="M:External.Type.Missing"/><see/>
              <list type="bullet"><item><description>BULLET</description></item></list>
              <list type="number"><item><description>NUMBER</description></item></list>
              <list type="table"><item><term>TERM</term><description>CELL</description></item></list>
              <list><item><term>DEFINITION</term><description>DETAIL</description></item></list>
              <unknown>IGNORED_UNKNOWN</unknown>
            </summary>
            """));
        new SiteGenerator().Generate(model, workspace.Templates, workspace.Output);
        using var html = new HtmlParser().ParseDocument(File.ReadAllText(Path.Combine(workspace.Output, "Fixture", "First.htm")));
        var summary = html.QuerySelector("#summary")!;
        Assert.Contains("Paragraph & text", summary.TextContent);
        Assert.Equal("input", summary.QuerySelector("var")!.TextContent);
        Assert.Contains(summary.QuerySelectorAll("code"), c => c.TextContent == "<script>literal</script>");
        Assert.Contains(summary.QuerySelectorAll("code"), c => c.TextContent == "a < b");
        Assert.Null(summary.QuerySelector("script"));
        Assert.Equal("BULLET", summary.QuerySelector("ul li")!.TextContent);
        Assert.Equal("NUMBER", summary.QuerySelector("ol li")!.TextContent);
        Assert.Equal("CELL", summary.QuerySelector("table td:last-child")!.TextContent);
        Assert.Equal("DETAIL", summary.QuerySelector("dl dd")!.TextContent);
        Assert.Contains(summary.QuerySelectorAll("a"), a => a.TextContent == "Second");
        Assert.Contains(summary.QuerySelectorAll("a"), a => a.TextContent == "Same");
        Assert.Contains("String", summary.TextContent);
        Assert.Contains("Missing", summary.TextContent);
        Assert.DoesNotContain("IGNORED_UNKNOWN", summary.TextContent);
        Assert.Empty(HtmlSiteInspector.Inspect(workspace.Output).Failures);
    }

    [Theory]
    [InlineData("same")]
    [InlineData("inside")]
    [InlineData("outside")]
    [InlineData("root")]
    public void RejectsOverlappingTemplateOutputDirectoriesAndFilesystemRoot(string kind)
    {
        using var workspace = new TestWorkspace();
        string destination = kind switch
        {
            "same" => workspace.Templates,
            "inside" => Path.Combine(workspace.Templates, "site"),
            "outside" => workspace.Root,
            _ => Path.GetPathRoot(workspace.Root)!
        };
        var before = RenderingTests.Hashes(workspace.Root);
        Assert.Throws<RenderingException>(() => new SiteGenerator().Generate(TestWorkspace.ReadModel(), workspace.Templates, destination));
        Assert.Equal(before, RenderingTests.Hashes(workspace.Root));
    }

    [Fact]
    public void ResourceCollisionsAndEmptyTemplatesDoNotPublishPartialSites()
    {
        using var workspace = new TestWorkspace();
        var generator = new SiteGenerator();
        var model = TestWorkspace.ReadModel();
        File.WriteAllText(Path.Combine(workspace.Templates, "index.htm"), "COLLISION");
        Assert.Throws<RenderingException>(() => generator.Generate(model, workspace.Templates, workspace.Output));
        Assert.False(Directory.Exists(workspace.Output));
        string empty = Path.Combine(workspace.Root, "empty");
        Directory.CreateDirectory(empty);
        Assert.Throws<RenderingException>(() => generator.Generate(model, empty, workspace.Output));
        Assert.False(Directory.Exists(workspace.Output));
    }
}
