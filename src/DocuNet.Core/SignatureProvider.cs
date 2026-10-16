namespace DocuNet.Core;

internal readonly record struct GenericContext(
    ImmutableArray<TypeReference> TypeArguments,
    ImmutableArray<TypeReference> MethodArguments);

internal sealed class SignatureProvider : ISignatureTypeProvider<TypeReference, GenericContext>
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.Ordinal)
    {
        ["System.Void"] = "void", ["System.Boolean"] = "bool",
        ["System.Int16"] = "short",
        ["System.UInt16"] = "ushort", ["System.Int32"] = "int", ["System.UInt32"] = "uint",
        ["System.Int64"] = "long", ["System.UInt64"] = "ulong", ["System.Single"] = "float",
        ["System.Double"] = "double", ["System.Decimal"] = "decimal", ["System.String"] = "string",
        ["System.Object"] = "object"
    };

    public static TypeReference Named(string fullName, string? shortName = null) =>
        new(fullName, Aliases.GetValueOrDefault(fullName, shortName ?? fullName.Split('.').Last()), fullName);

    public TypeReference GetArrayType(TypeReference elementType, ArrayShape shape)
    {
        string xmlSuffix = "[" + string.Join(",", Enumerable.Repeat("0:", shape.Rank)) + "]";
        return new(elementType.XmlName + xmlSuffix,
            WrapperName(elementType) + "[" + new string(',', shape.Rank - 1) + "]", "");
    }

    public TypeReference GetByReferenceType(TypeReference elementType) =>
        new(elementType.XmlName + "@", WrapperName(elementType) + "&", "");
    public TypeReference GetPointerType(TypeReference elementType) =>
        new(elementType.XmlName + "*", WrapperName(elementType) + "*", "");
    public TypeReference GetSZArrayType(TypeReference elementType) =>
        new(elementType.XmlName + "[]", WrapperName(elementType) + "[]", "");

    private static string WrapperName(TypeReference element) =>
        element.DefinitionName.Length > 0 ? element.Name : element.PrettyName;

    public TypeReference GetFunctionPointerType(MethodSignature<TypeReference> signature) =>
        throw new DocumentationException("Function-pointer signatures are not supported; no synthetic XML identifier was emitted.");

    public TypeReference GetGenericInstantiation(TypeReference genericType, ImmutableArray<TypeReference> typeArguments)
    {
        // XML documentation constructed types use braces without the definition's arity suffix.
        string stem = System.Text.RegularExpressions.Regex.Replace(genericType.XmlName, "`[0-9]+", "");
        string prettyStem = genericType.PrettyName.Split('`')[0];
        return new(stem + "{" + string.Join(",", typeArguments.Select(a => a.XmlName)) + "}",
            genericType.IsNested ? genericType.PrettyName : prettyStem + "<" + string.Join(", ", typeArguments.Select(a => a.PrettyName)) + ">",
            genericType.DefinitionName) { Arguments = typeArguments, IsNested = genericType.IsNested };
    }

    public TypeReference GetGenericMethodParameter(GenericContext context, int index) =>
        context.MethodArguments.IsDefaultOrEmpty || index >= context.MethodArguments.Length
            ? Parameter("T" + index, index, true) : context.MethodArguments[index];
    public TypeReference GetGenericTypeParameter(GenericContext context, int index) =>
        context.TypeArguments.IsDefaultOrEmpty || index >= context.TypeArguments.Length
            ? Parameter("T" + index, index, false) : context.TypeArguments[index];
    public TypeReference GetModifiedType(TypeReference modifier, TypeReference unmodifiedType, bool isRequired) => unmodifiedType;
    public TypeReference GetPinnedType(TypeReference elementType) => elementType;

    public TypeReference GetPrimitiveType(PrimitiveTypeCode typeCode) => Named("System." + typeCode);

    public TypeReference GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) =>
        Named(FullName(reader, handle), reader.GetString(reader.GetTypeDefinition(handle).Name))
            with { IsNested = !reader.GetTypeDefinition(handle).GetDeclaringType().IsNil };

    public TypeReference GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) =>
        Named(FullName(reader, handle), reader.GetString(reader.GetTypeReference(handle).Name))
            with { IsNested = reader.GetTypeReference(handle).ResolutionScope.Kind == HandleKind.TypeReference };

    public TypeReference GetTypeFromSpecification(MetadataReader reader, GenericContext context,
        TypeSpecificationHandle handle, byte rawTypeKind) =>
        reader.GetTypeSpecification(handle).DecodeSignature(this, context);

    internal TypeReference Decode(MetadataReader reader, EntityHandle handle, GenericContext context) => handle.Kind switch
    {
        HandleKind.TypeDefinition => GetTypeFromDefinition(reader, (TypeDefinitionHandle)handle, 0),
        HandleKind.TypeReference => GetTypeFromReference(reader, (TypeReferenceHandle)handle, 0),
        HandleKind.TypeSpecification => GetTypeFromSpecification(reader, context, (TypeSpecificationHandle)handle, 0),
        _ => throw new DocumentationException($"Unsupported type handle: {handle.Kind}.")
    };

    internal static TypeReference Parameter(string name, int index, bool method) =>
        new((method ? "``" : "`") + index, name, "");

    internal static string FullName(MetadataReader reader, TypeDefinitionHandle handle)
    {
        var definition = reader.GetTypeDefinition(handle);
        string name = reader.GetString(definition.Name);
        if (!definition.GetDeclaringType().IsNil)
            return FullName(reader, definition.GetDeclaringType()) + "." + name;
        string ns = reader.GetString(definition.Namespace);
        return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
    }

    internal static string FullName(MetadataReader reader, TypeReferenceHandle handle)
    {
        var reference = reader.GetTypeReference(handle);
        string name = reader.GetString(reference.Name);
        if (reference.ResolutionScope.Kind == HandleKind.TypeReference)
            return FullName(reader, (TypeReferenceHandle)reference.ResolutionScope) + "." + name;
        string ns = reader.GetString(reference.Namespace);
        return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
    }
}
