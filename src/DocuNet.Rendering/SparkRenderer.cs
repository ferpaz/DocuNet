using Spark;
using Spark.Bindings;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Parser.Syntax;

namespace DocuNet.Rendering;

internal sealed class SparkRenderer
{
    private readonly SparkViewEngine engine;

    public SparkRenderer(string templateRoot)
    {
        var settings = new SparkSettings().SetBaseClassTypeName(typeof(TemplateView))
            .AddAssembly(typeof(TemplateView).Assembly).AddAssembly(typeof(DocumentationModel).Assembly)
            .AddNamespace("System.Linq").AddNamespace("DocuNet.Core").AddNamespace("DocuNet.Rendering")
            .SetAutomaticEncoding(false);
        var batch = new RoslynBatchCompiler(settings);
        var partial = new DefaultPartialProvider();
        engine = new SparkViewEngine(settings,
            new DefaultSyntaxProvider(new Spark.Parser.ParserSettings { AutomaticEncoding = false }),
            new DefaultViewActivator(), new DefaultLanguageFactory(batch, settings), new CompiledViewHolder(),
            new FileSystemViewFolder(templateRoot), batch, partial, new DefaultPartialReferenceProvider(partial),
            new DefaultBindingProvider(), null!);
    }

    public string Render(string template, string outputPath, ViewData data)
    {
        var view = (TemplateView)engine.CreateInstance(new SparkViewDescriptor().AddTemplate(template));
        try
        {
            view.ViewData = data;
            view.RelativeOutputPath = outputPath;
            using var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            view.RenderView(writer);
            return writer.ToString();
        }
        finally { engine.ReleaseInstance(view); }
    }
}
