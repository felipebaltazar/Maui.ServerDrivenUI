namespace Maui.ServerDrivenUI.ApiSample;

public class ServerUIElement
{
    #region Properties

    /// <summary>
    /// Unique key to map your Maui Visual Element to the server json
    /// </summary>
    public string Key
    {
        get; set;
    }

    /// <summary>
    /// Maui element type
    /// </summary>
    public string Type
    {
        get; set;
    }

    /// <summary>
    /// Class with namespace eg.: "MyNamespace.Folder.ClassName"
    /// </summary>
    public string Class
    {
        get; set;
    }

    /// <summary>
    /// Visual element properties
    /// </summary>
    public Dictionary<string, string> Properties
    {
        get; set;
    }
        = [];

    /// <summary>
    /// Custom namespaces to be used as root element
    /// </summary>
    public IList<CustomNamespace> CustomNamespaces
    {
        get; set;
    }

    /// <summary>
    /// Inner visual element content
    /// </summary>
    public IList<ServerUIElement> Content
    {
        get; set;
    }

    #endregion

    #region Constructors

    public ServerUIElement()
    {
        CustomNamespaces = new List<CustomNamespace>(0);
        Content = new List<ServerUIElement>(0);
        Class = string.Empty;
        Type = string.Empty;
        Key = string.Empty;
    }

    #endregion
}
