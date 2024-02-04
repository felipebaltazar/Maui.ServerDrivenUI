using Maui.ServerDrivenUI.Services;
using System.Text.Json.Serialization;

namespace Maui.ServerDrivenUI;

/// <summary>
/// Represents a MAUI visual element
/// </summary>
public class ServerUIElement
{
    #region Properties

    /// <summary>
    /// Unique key to map your Maui Visual Element to the server json
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Maui element type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Class with namespace eg.: "MyNamespace.Folder.ClassName"
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Visual element properties
    /// </summary>
    public Dictionary<string, string> Properties { get; set; }
        = [];

    /// <summary>
    /// Custom namespaces to be used as root element
    /// </summary>
    public IList<CustomNamespace> CustomNamespaces { get; set; }

    /// <summary>
    /// Inner visual element content
    /// </summary>
    public IList<ServerUIElement> Content { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Represents a MAUI visual element
    /// </summary>
    /// <param name="elementKey">Unique key to map your Maui Visual Element to the server json</param>
    /// <param name="type">Maui element type</param>
    /// <param name="class">Class with namespace eg.: "MyNamespace.Folder.ClassName"</param>
    /// <param name="content">Inner visual element content</param>
    /// <param name="customNamespaces">Custom namespaces to be used as root element</param>
    public ServerUIElement(
        string elementKey,
        string type,
        string @class,
        IList<ServerUIElement> content,
        IList<CustomNamespace>? customNamespaces = null)
    {
        CustomNamespaces = customNamespaces ?? new List<CustomNamespace>(0);
        Content = content;
        Class = @class;
        Type = type;
        Key = elementKey;
    }

    [JsonConstructor]
    [Obsolete("This constructor should only be used from serializer")]
    public ServerUIElement()
    {
        CustomNamespaces = new List<CustomNamespace>(0);
        Content = new List<ServerUIElement>(0);
        Class = string.Empty;
        Type = string.Empty;
        Key = string.Empty;
    }

    #endregion

    #region Public Methods

    public string ToXaml() =>
        XamlConverterService.ConvertToXml(this);

    #endregion
}
