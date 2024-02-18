using Maui.ServerDrivenUI.Services;

namespace Maui.ServerDrivenUI.Models;

internal sealed class UIElementResolver(Func<string, IServiceProvider, Task<ServerUIElement>> uiElementGetter) : IUIElementResolver
{
    private readonly Func<string, IServiceProvider, Task<ServerUIElement>> _uiElementGetter = uiElementGetter;

    public Task<ServerUIElement> GetElementAsync(string elementKey) =>
        _uiElementGetter(elementKey, ServiceProviderHelper.ServiceProvider!);
}
