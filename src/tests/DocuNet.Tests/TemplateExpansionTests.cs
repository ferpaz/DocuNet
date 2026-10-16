namespace DocuNet.Tests;

public sealed class TemplateExpansionTests
{
    [Theory]
    [InlineData("index.htm.spark", 1, "index.htm")]
    [InlineData("!namespace/index.htm.spark", 1, "Fixture/index.htm")]
    [InlineData("!namespace/!type.htm.spark", 10, "Fixture/First.htm")]
    [InlineData("!type.htm.spark", 10, "Fixture.First.htm")]
    [InlineData("!type/index.htm.spark", 10, "Fixture.First/index.htm")]
    [InlineData("!namespace/!type/index.htm.spark", 10, "Fixture/Fixture.First/index.htm")]
    [InlineData("!namespaceExtra/index.htm.spark", 1, "!namespaceExtra/index.htm")]
    public void ExpandsSpecialNamesAndKeepsLiteralDirectories(string template, int count, string expectedPath)
    {
        var outputs = TemplateExpander.Expand(template, TestWorkspace.ReadModel()).ToArray();
        Assert.Equal(count, outputs.Length);
        Assert.Contains(outputs, o => o.OutputPath == expectedPath);
    }

    [Theory]
    [InlineData("_partial.spark")]
    [InlineData("layouts/master.spark")]
    public void DoesNotEmitPartialsOrLayouts(string template) =>
        Assert.Empty(TemplateExpander.Expand(template, TestWorkspace.ReadModel()));
}
