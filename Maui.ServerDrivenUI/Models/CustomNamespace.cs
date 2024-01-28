namespace Maui.ServerDrivenUI;

/// <summary>
/// Represents a xaml namespace eg.: xmlns:alias="clr-namespace:namespace;assembly=assembly"
/// </summary>
/// <param name="alias">Define the alias from custom namespaces</param>
/// <param name="namespace">clr-namespace</param>
/// <param name="assembly">Assembly where the custom control is located</param>
public class CustomNamespace(string alias, string @namespace, string? assembly = null)
{
    public string Alias { get; private set; } = alias;
    public string Namespace { get; private set; } = @namespace;
    public string? Assembly { get; private set; } = assembly;
}
