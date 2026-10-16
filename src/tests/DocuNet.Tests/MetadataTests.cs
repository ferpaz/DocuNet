using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DocuNet.Tests;

public sealed class MetadataTests
{
    [Fact]
    public void ReadsPublicMembersAndXmlWithoutExecutingModuleOrAttributes()
    {
        var model = TestWorkspace.ReadModel();
        var first = Assert.Single(model.Types, t => t.FullName == "Fixture.First");
        var second = Assert.Single(model.Types, t => t.FullName == "Fixture.Second");
        Assert.Contains(first.Methods, m => m.Name == "Same" && m.Summary.Element!.Value.Contains("FIRST"));
        Assert.Contains(second.Methods, m => m.Name == "Same" && m.Summary.Element!.Value.Contains("SECOND"));
        Assert.DoesNotContain(model.Types, t => t.Name == "HiddenType");
        Assert.DoesNotContain(first.Methods, m => m.Name is ".ctor" or "Hidden");
        Assert.Contains(first.Methods, m => m.Name == "NoXml" && m.Summary.IsEmpty);
        Assert.Contains(first.Methods, m => m.Name == "ToString" && m.IsInherited);
        Assert.DoesNotContain(AppDomain.CurrentDomain.GetAssemblies(), a => a.GetName().Name == "DocuNet.TestFixtures");
        Assert.Equal("FixtureProduct", model.Assemblies[0].ProductName);
        Assert.DoesNotContain(model.Types.Single(t => t.Name == "Extensions").Methods, m => m.IsInherited);
        Assert.Contains(model.Types.Single(t => t.Name == "Status").Fields, f => f.Name == "value__");
    }

    [Fact]
    public void PreservesGenericOverloadIndexerByrefAndArrayXmlIdentifiers()
    {
        var model = TestWorkspace.ReadModel();
        var first = Assert.Single(model.Types, t => t.FullName == "Fixture.First");
        Assert.Contains(first.Methods, m => m.XmlId == "M:Fixture.First.Echo``1(``0)");
        Assert.Equal(2, first.Methods.Count(m => m.Name == "Find"));
        var container = Assert.Single(model.Types, t => t.FullName == "Fixture.Container`1");
        Assert.Contains(container.Properties, p => p.XmlId == "P:Fixture.Container`1.Item(System.Int32)");
        Assert.Contains(container.Methods, m =>
            m.XmlId == "M:Fixture.Container`1.Complex(`0@,System.Int32@,System.Int32[0:,0:],`0[])");
        var child = Assert.Single(model.Types, t => t.FullName == "Fixture.Child");
        Assert.Equal("string", Assert.Single(child.Properties, p => p.Name == "Value").ReturnType.PrettyName);
        Assert.True(Assert.Single(child.Properties, p => p.Name == "Value").Summary.IsEmpty);
        Assert.True(Assert.Single(model.Types, t => t.Name == "Extensions").Methods.Single(m => m.Name == "Describe").IsExtension);
        Assert.Contains(model.Types, t => t.FullName == "Fixture.Container`1.Status" && t.FileName == "Status.htm");
    }

    [Theory]
    [InlineData("<doc><members>")]
    [InlineData("<!DOCTYPE doc [<!ENTITY x SYSTEM 'file:///etc/passwd'>]><doc><members/></doc>")]
    [InlineData("<doc><members><member name='T:X'/><member name='T:X'/></members></doc>")]
    public void RejectsMalformedDtdAndDuplicateXml(string contents)
    {
        using var workspace = new TestWorkspace();
        string path = Path.Combine(workspace.Root, "bad.xml");
        File.WriteAllText(path, contents);
        Assert.Throws<DocumentationException>(() => XmlDocumentation.Read([path]));
    }

    [Fact]
    public void MissingBaseDependencyProducesDiagnosticInsteadOfPartialSuccess()
    {
        using var workspace = new TestWorkspace();
        string dependency = Path.Combine(workspace.Root, "MissingBase.dll");
        string consumer = Path.Combine(workspace.Root, "Consumer.dll");
        Compile("MissingBase", "namespace Missing { public class Base {} }", dependency);
        Compile("Consumer", "namespace Example { public class Child : Missing.Base {} }", consumer, dependency);
        File.Delete(dependency);
        string xml = Path.Combine(workspace.Root, "Consumer.xml");
        File.WriteAllText(xml, "<doc><members/></doc>");
        var failure = Assert.Throws<DocumentationException>(() => new MetadataDocumentationReader().Read([consumer], [xml]));
        Assert.Contains("Missing.Base", failure.Message);
    }

    [Fact]
    public void FrameworkInheritanceUsesFrameworkReferenceMetadata()
    {
        using var workspace = new TestWorkspace();
        string legacy = Path.Combine(workspace.Root, "Legacy.dll");
        string modern = Path.Combine(workspace.Root, "Modern.dll");
        string xml = Path.Combine(workspace.Root, "Empty.xml");
        File.WriteAllText(xml, "<doc><members/></doc>");
        const string source = "namespace Example { public class Child : System.Collections.Generic.List<int> {} }";
        string reference = Path.Combine(AppContext.BaseDirectory, "ReferenceAssemblies", "net48", "mscorlib.dll");
        var result = CSharpCompilation.Create("Legacy", [CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken)],
            [MetadataReference.CreateFromFile(reference)], new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)).Emit(legacy, cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        Compile("Modern", source, modern);
        var reader = new MetadataDocumentationReader();
        Assert.DoesNotContain(Assert.Single(reader.Read([legacy], [xml]).Types).Methods, m => m.Name == "EnsureCapacity");
        Assert.Contains(Assert.Single(reader.Read([modern], [xml]).Types).Methods, m => m.Name == "EnsureCapacity");
    }
    internal static void Compile(string name, string source, string path, params string[] extraReferences)
    {
        var platform = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var references = platform.Concat(extraReferences).Select(p => MetadataReference.CreateFromFile(p));
        var result = CSharpCompilation.Create(name, [CSharpSyntaxTree.ParseText(source)], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)).Emit(path);
        Assert.True(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
    }
}
