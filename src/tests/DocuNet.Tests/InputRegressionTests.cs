namespace DocuNet.Tests;

public sealed class InputRegressionTests
{
    [Fact]
    public void ReadsEventsNestedNameCollisionsAndGlobalTypesWithoutLoadingAssembly()
    {
        using var workspace = new TestWorkspace();
        string assembly = Path.Combine(workspace.Root, "Shapes.dll");
        string xml = Path.Combine(workspace.Root, "Shapes.xml");
        MetadataTests.Compile("Shapes", """
            public class GlobalType {}
            namespace Shapes {
                public interface IEvents { event System.EventHandler Changed; }
                public abstract class Events : IEvents { public abstract event System.EventHandler Changed; }
                public class Left { public class Shared {} private class Hidden {} }
                public class Right { public class Shared {} }
            }
            """, assembly);
        File.WriteAllText(xml, """
            <doc><members>
              <member name="E:Shapes.Events.Changed"><summary>EVENT DOC</summary></member>
              <member name="T:Shapes.Left.Shared"><summary>LEFT NESTED</summary></member>
            </members></doc>
            """);
        var model = new MetadataDocumentationReader().Read([assembly], [xml]);
        var ev = Assert.Single(model.Types.Single(t => t.FullName == "Shapes.Events").Events);
        Assert.Equal("E:Shapes.Events.Changed", ev.XmlId);
        Assert.Equal("System.EventHandler", ev.ReturnType.DefinitionName);
        Assert.Equal("EVENT DOC", ev.Summary.Element!.Value);
        Assert.Contains(model.Namespaces, n => n.Name == "(global)");
        var nested = model.Types.Where(t => t.Name == "Shared").ToArray();
        Assert.Equal(2, nested.Length);
        Assert.Equal(2, nested.Select(t => t.FileName).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Contains(nested, t => t.FullName == "Shapes.Left.Shared" && t.Summary.Element!.Value == "LEFT NESTED");
        Assert.DoesNotContain(model.Types, t => t.Name == "Hidden");
        Assert.DoesNotContain(AppDomain.CurrentDomain.GetAssemblies(), a => a.GetName().Name == "Shapes");
    }

    [Fact]
    public void RejectsDuplicateTypesAcrossAssemblies()
    {
        using var workspace = new TestWorkspace();
        string a = Path.Combine(workspace.Root, "A.dll"), b = Path.Combine(workspace.Root, "B.dll");
        MetadataTests.Compile("A", "public class Duplicate {}", a);
        MetadataTests.Compile("B", "public class Duplicate {}", b);
        var failure = Assert.Throws<DocumentationException>(() => new MetadataDocumentationReader().Read([a, b], []));
        Assert.Contains("Duplicate input type", failure.Message);
    }

    [Theory]
    [InlineData("<wrong/>")]
    [InlineData("<doc/>")]
    [InlineData("<doc><members><member/></members></doc>")]
    public void RejectsXmlWithoutRequiredStructureOrMemberIdentifiers(string text)
    {
        using var workspace = new TestWorkspace();
        string xml = Path.Combine(workspace.Root, "invalid.xml");
        File.WriteAllText(xml, text);
        Assert.Throws<DocumentationException>(() => XmlDocumentation.Read([xml]));
    }

    [Fact]
    public void ReportsMissingXmlInvalidPeAndUnmatchedPatternsWithoutSuccess()
    {
        using var workspace = new TestWorkspace();
        string assembly = Path.Combine(workspace.Root, "NoXml.dll");
        File.Copy(TestWorkspace.AssemblyPath, assembly);
        using var output = new StringWriter();
        using var error = new StringWriter();
        Assert.Equal(2, CliApplication.Run([assembly], output, error));
        File.WriteAllText(assembly, "Not a PE assembly");
        Assert.Equal(3, CliApplication.Run([assembly, TestWorkspace.XmlPath], output, error));
        Assert.Equal(2, CliApplication.Run([Path.Combine(workspace.Root, "Missing*.dll")], output, error));
        Assert.Equal(2, CliApplication.Run([TestWorkspace.XmlPath], output, error));
        Assert.Equal(2, CliApplication.Run(["unsupported.txt"], output, error));
        Assert.DoesNotContain("Generated", output.ToString());
        Assert.NotEmpty(error.ToString());
    }

    [Fact]
    public void ExpandsWildcardInputsAndDeduplicatesRepeatedAssemblyPaths()
    {
        using var workspace = new TestWorkspace();
        string assembly = Path.Combine(workspace.Root, "Library.dll");
        File.Copy(TestWorkspace.AssemblyPath, assembly);
        File.Copy(TestWorkspace.XmlPath, Path.Combine(workspace.Root, "Library.xml"));
        using var output = new StringWriter();
        using var error = new StringWriter();
        int exit = CliApplication.Run([Path.Combine(workspace.Root, "*.dll"), assembly,
            "--templates=" + workspace.Templates, "--output=" + workspace.Output], output, error);
        Assert.Equal(0, exit);
        Assert.Contains("10 types", output.ToString());
        Assert.Empty(error.ToString());
    }
}
