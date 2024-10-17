using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Maui.ServerDrivenUI.Services;

internal static class XamlConverterService
{
    internal static Dictionary<string, FormattedString> LabelsSpans = new Dictionary<string, FormattedString>();

    public static string ConvertToXml(ServerUIElement element)
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append($"<{element.Type}");
        strBuilder.AppendLine();

        if (!string.IsNullOrWhiteSpace(element.Class))
            strBuilder.AppendLine($"x:Class=\"{element.Class}\"");

        strBuilder.AppendLine($"xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
        strBuilder.AppendLine($"xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"");

        foreach (var cn in element.CustomNamespaces)
            strBuilder.AppendLine($"xmlns:{cn.Alias}=\"clr-namespace:{cn.Namespace}{ParseAssembly(cn)}\"");

        var contentPropertiesBuilder = ParseProperties(element, strBuilder);

        strBuilder.Append('>');
        strBuilder.AppendLine();

        strBuilder.AppendJoin('\n', element.Content.Select(c => c.ToXaml(element.CustomNamespaces)));
        strBuilder.AppendLine();
        strBuilder.Append(contentPropertiesBuilder);

        strBuilder.AppendLine();
        strBuilder.Append($"</{element.Type}>");
        return strBuilder.ToString();
    }

    private static StringBuilder ParseProperties(ServerUIElement element, StringBuilder strBuilder)
    {
        var contentPropertiesBuilder = new StringBuilder();
        foreach (var prop in element.Properties)
        {
            if (prop.Value is JsonValue jv)
            {
                var valueStr = jv.GetValue<string>();
                var teste2 = jv.ToString();
                strBuilder.AppendLine($"{prop.Key}=\"{valueStr}\"");
            }
            else if (prop.Value is JsonArray array)
            {
                contentPropertiesBuilder.AppendLine($"<{element.Type}.{prop.Key}>");

                foreach (var innerJNode in array.Where(n => n != null))
                {
                    var innerElementJson = innerJNode!.ToString();
                    var innerXaml = GetInnerXaml(innerElementJson, element.CustomNamespaces);

                    contentPropertiesBuilder.AppendLine(innerXaml);
                }

                contentPropertiesBuilder.AppendLine($"</{element.Type}.{prop.Key}>");
            }
            else
            {
                if (prop.Key.Equals("FormattedText", StringComparison.OrdinalIgnoreCase))
                {
                    FormattedStringWorkArround(prop.Value, element, strBuilder);
                }
                else
                {
                    var innerElementJson = prop.Value.ToString();
                    var innerXaml = GetInnerXaml(innerElementJson, element.CustomNamespaces);

                    contentPropertiesBuilder.AppendLine($"<{element.Type}.{prop.Key}>");
                    contentPropertiesBuilder.AppendLine(innerXaml);
                    contentPropertiesBuilder.AppendLine($"</{element.Type}.{prop.Key}>");
                }
            }
        }

        return contentPropertiesBuilder;
    }

    private static string GetInnerXaml(string innerElementJson, IList<CustomNamespace> extraCustomNamespaces)
    {
        var innerElement = ParseUIElement(innerElementJson, extraCustomNamespaces);
        var innerXaml = ConvertToXml(innerElement!);
        return innerXaml;
    }

    private static ServerUIElement? ParseUIElement(string innerElementJson, IList<CustomNamespace> extraCustomNamespaces)
    {
        var innerElement = JsonSerializer.Deserialize<ServerUIElement>(innerElementJson);

        if (extraCustomNamespaces?.Count > 0 && innerElement != null)
        {
            foreach (var customNamespace in extraCustomNamespaces)
            {
                if (!innerElement.CustomNamespaces.Contains(customNamespace))
                    innerElement.CustomNamespaces.Add(customNamespace);
            }
        }

        return innerElement;
    }

    private static string ParseAssembly(CustomNamespace custom)
    {
        if (string.IsNullOrWhiteSpace(custom.Assembly))
            return string.Empty;

        return $";assembly={custom.Assembly}";
    }

    //HACK: This is a workaround to handle FormattedString, because it is not supported by XAML loader.
    private static void FormattedStringWorkArround(JsonNode objectNode, ServerUIElement element, StringBuilder strBuilder)
    {
        var formattedString = new FormattedString();
        var innerElementJson = objectNode.ToString();
        var innerElement = ParseUIElement(innerElementJson, element.CustomNamespaces);

        if ((innerElement?.Properties.TryGetValue("Spans", out var spans) ?? false) && spans is JsonArray array)
        {
            foreach (var innerJNode in array.Where(n => n != null))
            {
                var innerNodeJson = innerJNode!.ToString();

                var span = new Span();
                var spanType = typeof(Span);
                var uIElement = ParseUIElement(innerNodeJson, element.CustomNamespaces);
                foreach (var propName in uIElement?.Properties ?? [])
                {
                    var property = spanType.GetProperties()
                                           .FirstOrDefault(p => p.Name.Equals(propName.Key, StringComparison.OrdinalIgnoreCase));

                    SetPropertyValue(property, propName.Value.ToString(), span, element.CustomNamespaces);
                }

                formattedString.Spans.Add(span);
            }
        }

        if (formattedString.Spans.Count > 0)
        {
            var elementName = element.Properties.FirstOrDefault(p => p.Key.Equals("x:Name", StringComparison.OrdinalIgnoreCase)).Value?.ToString();
            if (string.IsNullOrWhiteSpace(elementName))
            {
                elementName = Guid.NewGuid().ToString().Replace("-", string.Empty);
                strBuilder.AppendLine($"x:Name=\"{elementName}\"");
            }

            _ = LabelsSpans.TryAdd(elementName!, formattedString);
        }
    }

    private static void SetPropertyValue(PropertyInfo? property, string value, object instance, IList<CustomNamespace> customNamespaces)
    {
        if (property == null)
            return;

        if (value.StartsWith("{"))
        {
            value = value.TrimStart('{').TrimEnd('}');
            if (value.StartsWith("Binding"))
            {
                value = value.Replace("Binding ", string.Empty);

                var separator = value.IndexOf(",");
                if (separator >= -1)
                {
                    value = value[..separator];
                }
            }
            else if (value.StartsWith("x:Static"))
            {
                value = value.Replace("x:Static ", string.Empty);
                var keys = value.Split(':');
                var alias = keys[0];
                var memberClass = keys[1].Split('.');
                var className = memberClass[0];
                var member = memberClass[1];

                var customNamespace = customNamespaces.FirstOrDefault(c => c.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase));
                var assembly = customNamespace?.Assembly != null
                    ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == customNamespace.Assembly)
                    : Assembly.GetAssembly(Application.Current!.GetType());

                var type = assembly?.GetType($"{customNamespace?.Namespace}.{className}");
                if (type != null)
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                    BindingFlags.FlattenHierarchy)
                        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                        .ToList();

                    var field = fields.FirstOrDefault(f => f.Name.Equals(member, StringComparison.OrdinalIgnoreCase));
                    var constantValue = field?.GetValue(null);
                    property.SetValue(instance, constantValue);
                }
            }
        }
        else if (Enum.TryParse(property.PropertyType.Name, true, out TypeCode enumValue))
        {
            var convertedValue = Convert.ChangeType(value, enumValue);
            property.SetValue(instance, convertedValue);
        }
    }
}
