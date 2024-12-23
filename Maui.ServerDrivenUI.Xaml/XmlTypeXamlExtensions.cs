namespace Maui.ServerDrivenUI.Xaml;

static class XmlTypeXamlExtensions
{
    public static T? GetTypeReference<T>(
        this XmlType xmlType,
        IEnumerable<XmlnsDefinitionAttribute> xmlnsDefinitions,
        string defaultAssemblyName,
        Func<(string typeName, string clrNamespace, string assemblyName), T> refFromTypeInfo)
        where T : class
    {
        var lookupAssemblies = new List<XmlnsDefinitionAttribute>();
        var namespaceURI = xmlType.NamespaceUri;
        var elementName = xmlType.Name;
        var typeArguments = xmlType.TypeArguments;

        foreach (var xmlnsDef in xmlnsDefinitions)
        {
            if (xmlnsDef.XmlNamespace != namespaceURI)
                continue;
            lookupAssemblies.Add(xmlnsDef);
        }

        if (lookupAssemblies.Count == 0)
        {
            XmlnsHelper.ParseXmlns(namespaceURI, out _, out var ns, out var asmstring, out _);
            asmstring ??= defaultAssemblyName;
            if (namespaceURI != null && ns != null)
                lookupAssemblies.Add(new XmlnsDefinitionAttribute(namespaceURI, ns) { AssemblyName = asmstring });
        }

        var lookupNames = new List<string>(capacity: 2);
        if (elementName != "DataTemplate" && !elementName.EndsWith("Extension", StringComparison.Ordinal))
            lookupNames.Add(elementName + "Extension");
        lookupNames.Add(elementName);

        for (var i = 0; i < lookupNames.Count; i++)
        {
            var name = lookupNames[i];
            var lastIndex = name.LastIndexOf(":", StringComparison.Ordinal);
            if (lastIndex != -1)
                name = name.Substring(lastIndex + 1);
            if (typeArguments != null)
                name += "`" + typeArguments.Count; //this will return an open generic Type
            lookupNames[i] = name;
        }

        var potentialTypes = new List<(string typeName, string clrNamespace, string assemblyName)>();
        foreach (string typeName in lookupNames)
        {
            foreach (XmlnsDefinitionAttribute xmlnsDefinitionAttribute in lookupAssemblies)
            {
                potentialTypes.Add(new(typeName, xmlnsDefinitionAttribute.ClrNamespace, xmlnsDefinitionAttribute.AssemblyName));

                // As a fallback, for assembly=mscorlib try assembly=System.Private.CoreLib
                if (xmlnsDefinitionAttribute.AssemblyName is string assemblyName &&
                    (assemblyName == "mscorlib" || assemblyName.StartsWith("mscorlib,", StringComparison.Ordinal)))
                {
                    potentialTypes.Add(new(typeName, xmlnsDefinitionAttribute.ClrNamespace, "System.Private.CoreLib"));
                }
            }
        }

        T? type = null;
        foreach (var typeInfo in potentialTypes)
            if ((type = refFromTypeInfo(typeInfo)) != null)
                break;

        return type;
    }
}
