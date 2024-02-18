using System.Text.Json.Serialization;

namespace Maui.ServerDrivenUI;

/// <summary>
/// Represents a xaml namespace eg.: xmlns:alias="clr-namespace:namespace;assembly=assembly"
/// </summary>
/// <param name="alias">Define the alias from custom namespaces</param>
/// <param name="namespace">clr-namespace</param>
/// <param name="assembly">Assembly where the custom control is located</param>
public class CustomNamespace : IEquatable<CustomNamespace>
{
    public string Alias
    {
        get; set;
    }

    public string Namespace
    {
        get; set;
    }

    public string? Assembly
    {
        get; set;
    }

    [JsonConstructor]
    [Obsolete("This constructor should only be used from serializer")]
    public CustomNamespace()
    {
        Alias = string.Empty;
        Namespace = string.Empty;
    }

    public CustomNamespace(string alias, string @namespace, string? assembly = null)
    {
        Alias = alias;
        Namespace = @namespace;
        Assembly = assembly;
    }

    public bool Equals(CustomNamespace? other)
    {
        if (other is null)
            return false;

        return Namespace == other?.Namespace 
            && Alias == other?.Alias
            && Assembly == other?.Assembly;
    }

    public override bool Equals(object? obj)
    {
        if(obj is CustomNamespace cn)
            return Equals(cn);

        return false;
    }

    public override int GetHashCode() =>
        (Namespace, Alias, Assembly).GetHashCode();
}
