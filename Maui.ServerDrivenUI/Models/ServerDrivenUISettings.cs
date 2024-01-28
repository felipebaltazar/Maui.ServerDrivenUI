
namespace Maui.ServerDrivenUI.Models;

internal sealed class ServerDrivenUISettings : IServerDrivenUISettings
{
    public IUIElementResolver? ElementResolver { get; private set; }

    public HashSet<string> CacheEntryKeys { get; } = [];

    public TimeSpan UIElementCacheExpiration { get; set; } = TimeSpan.FromDays(1);

    public void AddServerElement(string key)
    {
        if (!CacheEntryKeys.Add(key))
            throw new DependencyRegistrationException($"The key: '{key}' already has been registered");
    }

    public void RegisterElementGetter(Func<string, Task<ServerUIElement>> uiElementGetter) =>
        ElementResolver = new UIElementResolver(uiElementGetter);
}
