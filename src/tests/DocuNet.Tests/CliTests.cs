namespace DocuNet.Tests;

public sealed class CliTests
{
    [Fact]
    public void HelpWritesUsageAndDoesNotWriteDiagnostics()
    {
        var output = Substitute.For<TextWriter>();
        var error = Substitute.For<TextWriter>();
        Assert.Equal(0, CliApplication.Run(["--help"], output, error));
        output.Received().WriteLine(Arg.Is<string>(s => s.StartsWith("Usage:", StringComparison.Ordinal)));
        error.DidNotReceive().WriteLine(Arg.Any<string>());
    }

    [Theory]
    [InlineData("--bogus")]
    [InlineData("--output=")]
    [InlineData("--outputExtra=wrong")]
    [InlineData("missing.dll")]
    public void InvalidArgumentsReturnTwoAndNoSuccess(string argument)
    {
        var output = Substitute.For<TextWriter>();
        using var error = new StringWriter();
        Assert.Equal(2, CliApplication.Run([argument], output, error));
        Assert.NotEmpty(error.ToString());
        output.DidNotReceive().WriteLine(Arg.Is<string>(s => s.StartsWith("Generated", StringComparison.Ordinal)));
    }

    [Fact]
    public void MetadataAndTemplateFailuresHaveDistinctExitCodes()
    {
        using var workspace = new TestWorkspace();
        string xml = Path.Combine(workspace.Root, "invalid.xml");
        File.WriteAllText(xml, "<doc>");
        using var output = new StringWriter();
        using var error = new StringWriter();
        Assert.Equal(3, CliApplication.Run([TestWorkspace.AssemblyPath, xml], output, error));
        Assert.Equal(4, CliApplication.Run([TestWorkspace.AssemblyPath, TestWorkspace.XmlPath,
            "--templates=" + Path.Combine(workspace.Root, "missing"), "--output=" + workspace.Output], output, error));
        Assert.False(Directory.Exists(workspace.Output));
    }
    [Fact]
    public void UnwritableOutputParentHasRenderingExitCode()
    {
        using var workspace = new TestWorkspace();
        string blocker = Path.Combine(workspace.Root, "blocked");
        File.WriteAllText(blocker, "KEEP");
        using var output = new StringWriter();
        using var error = new StringWriter();
        Assert.Equal(4, CliApplication.Run([TestWorkspace.AssemblyPath, TestWorkspace.XmlPath,
            "--templates=" + workspace.Templates, "--output=" + Path.Combine(blocker, "site")], output, error));
        Assert.Equal("KEEP", File.ReadAllText(blocker));
    }
    [Fact]
    public void InfersUppercaseXmlCompanionOnCaseSensitiveFilesystems()
    {
        using var workspace = new TestWorkspace();
        string assembly = Path.Combine(workspace.Root, "Library.dll");
        File.Copy(TestWorkspace.AssemblyPath, assembly);
        File.Copy(TestWorkspace.XmlPath, Path.Combine(workspace.Root, "LIBRARY.XML"));
        using var output = new StringWriter();
        using var error = new StringWriter();
        Assert.Equal(0, CliApplication.Run([assembly, "--templates=" + workspace.Templates,
            "--output=" + workspace.Output], output, error));
        Assert.True(File.Exists(Path.Combine(workspace.Output, "index.htm")));
    }
}
