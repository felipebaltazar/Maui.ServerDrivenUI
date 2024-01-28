using System.Text;

namespace Maui.ServerDrivenUI.Services;

internal static class XamlConverterService
{
    public static string ConvertToXml(ServerUIElement element)
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append($"<{element.Type}");
        strBuilder.AppendLine();

        if (element.IsRootElement)
        {
            strBuilder.AppendLine($"x:Class=\"{element.Class}\"");
            strBuilder.AppendLine($"xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
            strBuilder.AppendLine($"xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"");

            foreach (var cn in element.CustomNamespaces)
                strBuilder.AppendLine($"xmlns:{cn.Alias}=\"clr-namespace:{cn.Namespace}{ParseAssembly(cn)}\"");
        }

        foreach (var prop in element.Properties)
            strBuilder.AppendLine($"{prop.Key}=\"{prop.Value}\"");

        strBuilder.Append('>');
        strBuilder.AppendLine();

        strBuilder.AppendJoin('\n', element.Content.Select(c => c.ToXaml()));

        strBuilder.AppendLine();
        strBuilder.Append($"</{element.Type}>");
        return strBuilder.ToString();
    }

    private static string ParseAssembly(CustomNamespace custom)
    {
        if (string.IsNullOrWhiteSpace(custom.Assembly))
            return string.Empty;

        return $";assembly={custom.Assembly}";
    }
}
