namespace Maui.ServerDrivenUI;

public interface IServerDrivenUISettings
{
    string? CacheFilePath { get; set; }

    HashSet<string> CacheEntryKeys { get; }

    IUIElementResolver? ElementResolver { get; }

    TimeSpan UIElementCacheExpiration { get; set; }

    void RegisterElementGetter(Func<string, IServiceProvider, Task<ServerUIElement>> UiElementGetter);

    void AddServerElement(string key);
}
