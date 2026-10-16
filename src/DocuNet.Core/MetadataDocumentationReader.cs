namespace DocuNet.Core;

/// <summary>Reads PE/ECMA-335 and XML only. Target code and attribute constructors are never loaded.</summary>
public sealed partial class MetadataDocumentationReader
{
    private readonly SignatureProvider signatures = new();

    public DocumentationModel Read(IEnumerable<string> assemblyPaths, IEnumerable<string> xmlPaths)
    {
        try
        {
            var xml = XmlDocumentation.Read(xmlPaths);
            using var workspace = new MetadataWorkspace(assemblyPaths);
            var model = new DocumentationModel();
            var sources = new List<(MetadataImage Image, TypeDefinitionHandle Handle, TypeDocumentation Type)>();
            foreach (var image in workspace.Inputs)
            {
                var reader = image.Reader;
                var assembly = reader.GetAssemblyDefinition();
                model.Assemblies.Add(new AssemblyDocumentation
                {
                    Name = image.Name, Version = assembly.Version, Path = image.Path,
                    ProductName = ReadStringAttribute(reader, assembly.GetCustomAttributes(), "System.Reflection.AssemblyProductAttribute"),
                    FileDescription = ReadStringAttribute(reader, assembly.GetCustomAttributes(), "System.Reflection.AssemblyTitleAttribute")
                });
                foreach (var handle in reader.TypeDefinitions)
                {
                    var definition = reader.GetTypeDefinition(handle);
                    string name = reader.GetString(definition.Name);
                    if (!IsExported(reader, handle) || name.StartsWith("__", StringComparison.Ordinal)
                        || (definition.Attributes & TypeAttributes.SpecialName) != 0) continue;
                    string fullName = SignatureProvider.FullName(reader, handle);
                    if (model.Types.Any(t => t.Id == "T:" + fullName))
                        throw new DocumentationException($"Duplicate input type: {fullName}.");
                    string nsName = GetNamespace(reader, handle);
                    var ns = model.Namespaces.FirstOrDefault(n => n.Name == nsName);
                    if (ns is null)
                    {
                        ns = new NamespaceDocumentation { Id = "N:" + nsName, Name = nsName, PrettyName = nsName, FullName = nsName };
                        model.Namespaces.Add(ns);
                    }
                    var context = GetContext(reader, definition);
                    string prettyName = name.Split('`')[0];
                    if (!context.TypeArguments.IsDefaultOrEmpty)
                        prettyName += "<" + string.Join(", ", context.TypeArguments.Select(p => p.PrettyName)) + ">";
                    var type = new TypeDocumentation
                    {
                        Id = "T:" + fullName, Name = name, PrettyName = prettyName, FullName = fullName,
                        FileName = name + ".htm", Namespace = ns,
                        IsInterface = (definition.Attributes & TypeAttributes.Interface) != 0,
                        IsStatic = (definition.Attributes & (TypeAttributes.Abstract | TypeAttributes.Sealed)) == (TypeAttributes.Abstract | TypeAttributes.Sealed),
                        ParentType = definition.BaseType.IsNil ? null : signatures.Decode(reader, definition.BaseType, context)
                    };
                    type.ReadComments(xml.Find(type.Id));
                    foreach (var ih in definition.GetInterfaceImplementations())
                        type.Interfaces.Add(signatures.Decode(reader, reader.GetInterfaceImplementation(ih).Interface, context));
                    ns.Types.Add(type);
                    sources.Add((image, handle, type));
                }
            }
            // Preserve historical short filenames where unambiguous; distinguish nested-name collisions deterministically.
            foreach (var ns in model.Namespaces)
                foreach (var group in ns.Types.GroupBy(t => t.FileName, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
                    foreach (var type in group)
                        type.FileName = type.FullName[(ns.Name == "(global)" ? 0 : ns.Name.Length + 1)..] + ".htm";
            foreach (var source in sources)
                ReadMembers(workspace, source.Image, source.Handle, source.Type, xml, [], false,
                    GetContext(source.Image.Reader, source.Image.Reader.GetTypeDefinition(source.Handle)), source.Image.IsFramework);
            foreach (var ns in model.Namespaces)
            {
                ns.Types.Sort((a, b) => StringComparer.Ordinal.Compare(a.Name, b.Name));
                foreach (var type in ns.Types)
                {
                    type.Methods.Sort((a, b) => Compare(a, b));
                    type.Properties.Sort((a, b) => Compare(a, b));
                    type.Fields.Sort((a, b) => Compare(a, b));
                    type.Events.Sort((a, b) => Compare(a, b));
                }
            }
            model.Namespaces.Sort((a, b) => StringComparer.Ordinal.Compare(a.Name, b.Name));
            return model;
        }
        catch (Exception ex) when (ex is BadImageFormatException or IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            throw new DocumentationException($"Cannot read documentation inputs: {ex.Message}", ex);
        }
    }

    private static int Compare(MemberDocumentation a, MemberDocumentation b)
    {
        int name = StringComparer.Ordinal.Compare(a.Name, b.Name);
        return name == 0 ? StringComparer.Ordinal.Compare(a.Id, b.Id) : name;
    }

    private static bool IsExported(MetadataReader reader, TypeDefinitionHandle handle)
    {
        var type = reader.GetTypeDefinition(handle);
        var visibility = type.Attributes & TypeAttributes.VisibilityMask;
        return visibility == TypeAttributes.Public || visibility == TypeAttributes.NestedPublic
            && !type.GetDeclaringType().IsNil && IsExported(reader, type.GetDeclaringType());
    }

    private static string GetNamespace(MetadataReader reader, TypeDefinitionHandle handle)
    {
        var type = reader.GetTypeDefinition(handle);
        if (!type.GetDeclaringType().IsNil) return GetNamespace(reader, type.GetDeclaringType());
        string ns = reader.GetString(type.Namespace);
        return ns.Length == 0 ? "(global)" : ns;
    }

    private static GenericContext GetContext(MetadataReader reader, TypeDefinition type,
        MethodDefinition? method = null) => new(
            type.GetGenericParameters().Select(h => reader.GetGenericParameter(h))
                .Select(p => SignatureProvider.Parameter(reader.GetString(p.Name), p.Index, false)).ToImmutableArray(),
            method?.GetGenericParameters().Select(h => reader.GetGenericParameter(h))
                .Select(p => SignatureProvider.Parameter(reader.GetString(p.Name), p.Index, true)).ToImmutableArray() ?? []);

    private static string AttributeType(MetadataReader reader, CustomAttributeHandle handle)
    {
        var attribute = reader.GetCustomAttribute(handle);
        EntityHandle parent = attribute.Constructor.Kind switch
        {
            HandleKind.MemberReference => reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor).Parent,
            HandleKind.MethodDefinition => reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor).GetDeclaringType(),
            _ => default
        };
        return parent.Kind switch
        {
            HandleKind.TypeReference => SignatureProvider.FullName(reader, (TypeReferenceHandle)parent),
            HandleKind.TypeDefinition => SignatureProvider.FullName(reader, (TypeDefinitionHandle)parent),
            _ => ""
        };
    }

    private static string ReadStringAttribute(MetadataReader reader, CustomAttributeHandleCollection handles, string name)
    {
        foreach (var handle in handles)
        {
            if (AttributeType(reader, handle) != name) continue;
            var blob = reader.GetBlobReader(reader.GetCustomAttribute(handle).Value);
            if (blob.ReadUInt16() != 1) throw new DocumentationException($"Malformed attribute: {name}.");
            return blob.ReadSerializedString() ?? "";
        }
        return "";
    }

    private static string ParameterSuffix(IEnumerable<TypeReference> parameters)
    {
        var list = parameters.Select(p => p.XmlName).ToArray();
        return list.Length == 0 ? "" : "(" + string.Join(",", list) + ")";
    }
}
