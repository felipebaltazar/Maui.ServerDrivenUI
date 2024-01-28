namespace Maui.ServerDrivenUI;

public interface IServerDrivenUISettings
{
    HashSet<string> CacheEntryKeys { get; }

    IUIElementResolver? ElementResolver { get; }

    TimeSpan UIElementCacheExpiration { get; set; }

    void RegisterElementGetter(Func<string, Task<ServerUIElement>> UiElementGetter);

    void AddServerElement(string key);
}
