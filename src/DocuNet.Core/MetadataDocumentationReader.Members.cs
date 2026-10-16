namespace DocuNet.Core;

public sealed partial class MetadataDocumentationReader
{
    private void ReadMembers(MetadataWorkspace workspace, MetadataImage image, TypeDefinitionHandle handle,
        TypeDocumentation owner, XmlDocumentation xml, HashSet<string> ancestry, bool inherited, GenericContext context, bool frameworkReferences)
    {
        var reader = image.Reader;
        var definition = reader.GetTypeDefinition(handle);
        string declaringName = SignatureProvider.FullName(reader, handle);
        if (!ancestry.Add(image.Path + ":" + declaringName))
            throw new DocumentationException($"Cyclic base type: {declaringName}.");
        foreach (var mh in definition.GetMethods())
        {
            var method = reader.GetMethodDefinition(mh);
            if ((method.Attributes & MethodAttributes.MemberAccessMask) != MethodAttributes.Public
                || (method.Attributes & MethodAttributes.SpecialName) != 0
                || inherited && (method.Attributes & MethodAttributes.Static) != 0) continue;
            var openContext = GetContext(reader, definition, method);
            var signature = method.DecodeSignature(signatures, context with { MethodArguments = openContext.MethodArguments });
            var sourceSignature = method.DecodeSignature(signatures, openContext);
            string rawName = reader.GetString(method.Name);
            string name = rawName + (signature.GenericParameterCount > 0 ? "``" + signature.GenericParameterCount : "");
            string sourceId = "M:" + declaringName + "." + name + ParameterSuffix(sourceSignature.ParameterTypes);
            string id = "M:" + owner.FullName + "." + name + ParameterSuffix(signature.ParameterTypes);
            if (owner.Methods.Any(m => m.Id == id)) continue;
            string prettyName = rawName + (signature.GenericParameterCount > 0
                ? "<" + string.Join(", ", openContext.MethodArguments.Select(t => t.PrettyName)) + ">" : "");
            var doc = new MethodDocumentation
            {
                Id = id, XmlId = sourceId, Name = name, PrettyName = prettyName, FullName = owner.FullName + "." + name,
                Type = owner, ReturnType = signature.ReturnType, IsInherited = inherited,
                IsStatic = (method.Attributes & MethodAttributes.Static) != 0,
                IsExtension = method.GetCustomAttributes().Any(h => AttributeType(reader, h) == "System.Runtime.CompilerServices.ExtensionAttribute")
            };
            var comments = xml.Find(inherited ? id : sourceId);
            doc.ReadComments(comments);
            doc.Returns = new(comments?.Element("returns"));
            var parameters = method.GetParameters().Select(reader.GetParameter)
                .Where(p => p.SequenceNumber > 0).ToDictionary(p => (int)p.SequenceNumber);
            for (int i = 0; i < signature.ParameterTypes.Length; i++)
            {
                string parameterName = parameters.TryGetValue(i + 1, out var parameter)
                    ? reader.GetString(parameter.Name) : "arg" + i;
                doc.Parameters.Add(new ParameterDocumentation
                {
                    Name = parameterName, Reference = signature.ParameterTypes[i],
                    Summary = new(comments?.Elements("param").FirstOrDefault(p => (string?)p.Attribute("name") == parameterName))
                });
            }
            owner.Methods.Add(doc);
        }
        foreach (var ph in definition.GetProperties())
        {
            var property = reader.GetPropertyDefinition(ph);
            var access = property.GetAccessors();
            if (!IsPublicAccessor(reader, access.Getter) && !IsPublicAccessor(reader, access.Setter)) continue;
            bool isStatic = IsStaticAccessor(reader, access.Getter.IsNil ? access.Setter : access.Getter);
            if (inherited && isStatic) continue;
            string name = reader.GetString(property.Name);
            var signature = property.DecodeSignature(signatures, context);
            var sourceSignature = property.DecodeSignature(signatures, GetContext(reader, definition));
            string id = "P:" + owner.FullName + "." + name + ParameterSuffix(signature.ParameterTypes);
            if (owner.Properties.Any(p => p.Id == id)) continue;
            string sourceId = "P:" + declaringName + "." + name + ParameterSuffix(sourceSignature.ParameterTypes);
            var doc = new PropertyDocumentation
            {
                Id = id, XmlId = sourceId, Name = name, PrettyName = name, FullName = owner.FullName + "." + name,
                Type = owner, ReturnType = signature.ReturnType, IsInherited = inherited, IsStatic = isStatic,
                HasGet = !access.Getter.IsNil, HasSet = !access.Setter.IsNil
            };
            doc.ReadComments(xml.Find(inherited ? id : sourceId));
            owner.Properties.Add(doc);
        }
        foreach (var fh in definition.GetFields())
        {
            var field = reader.GetFieldDefinition(fh);
            if ((field.Attributes & FieldAttributes.FieldAccessMask) != FieldAttributes.Public
                || inherited && (field.Attributes & FieldAttributes.Static) != 0) continue;
            string name = reader.GetString(field.Name);
            string id = "F:" + owner.FullName + "." + name;
            if (owner.Fields.Any(f => f.Id == id)) continue;
            string sourceId = "F:" + declaringName + "." + name;
            var doc = new FieldDocumentation
            {
                Id = id, XmlId = sourceId, Name = name, PrettyName = name, FullName = owner.FullName + "." + name,
                Type = owner, ReturnType = field.DecodeSignature(signatures, context), IsInherited = inherited,
                IsStatic = (field.Attributes & FieldAttributes.Static) != 0
            };
            doc.ReadComments(xml.Find(inherited ? id : sourceId));
            owner.Fields.Add(doc);
        }
        foreach (var eh in definition.GetEvents())
        {
            var ev = reader.GetEventDefinition(eh);
            var access = ev.GetAccessors();
            if (!IsPublicAccessor(reader, access.Adder) && !IsPublicAccessor(reader, access.Remover)) continue;
            bool isStatic = IsStaticAccessor(reader, access.Adder.IsNil ? access.Remover : access.Adder);
            if (inherited && isStatic) continue;
            string name = reader.GetString(ev.Name);
            string id = "E:" + owner.FullName + "." + name;
            if (owner.Events.Any(e => e.Id == id)) continue;
            string sourceId = "E:" + declaringName + "." + name;
            var doc = new EventDocumentation
            {
                Id = id, XmlId = sourceId, Name = name, PrettyName = name, FullName = owner.FullName + "." + name,
                Type = owner, ReturnType = signatures.Decode(reader, ev.Type, context),
                IsInherited = inherited, IsStatic = isStatic
            };
            doc.ReadComments(xml.Find(inherited ? id : sourceId));
            owner.Events.Add(doc);
        }
        if (!definition.BaseType.IsNil && !owner.IsStatic)
        {
            var parent = signatures.Decode(reader, definition.BaseType, context);
            var resolved = workspace.Resolve(parent.DefinitionName, frameworkReferences);
            var parentContext = parent.Arguments.Count > 0
                ? new GenericContext(parent.Arguments.ToImmutableArray(), [])
                : GetContext(resolved.Image.Reader, resolved.Image.Reader.GetTypeDefinition(resolved.Handle));
            ReadMembers(workspace, resolved.Image, resolved.Handle, owner, xml, ancestry, true, parentContext, frameworkReferences);
        }
        ancestry.Remove(image.Path + ":" + declaringName);
    }

    private static bool IsPublicAccessor(MetadataReader reader, MethodDefinitionHandle handle) =>
        !handle.IsNil && (reader.GetMethodDefinition(handle).Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
    private static bool IsStaticAccessor(MetadataReader reader, MethodDefinitionHandle handle) =>
        !handle.IsNil && (reader.GetMethodDefinition(handle).Attributes & MethodAttributes.Static) != 0;
}
