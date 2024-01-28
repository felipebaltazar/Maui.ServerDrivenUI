namespace Maui.ServerDrivenUI.Models;

internal sealed class UIElementResolver(Func<string, Task<ServerUIElement>> uiElementGetter) : IUIElementResolver
{
    private readonly Func<string, Task<ServerUIElement>> _uiElementGetter = uiElementGetter;

    public Task<ServerUIElement> GetElementAsync(string elementKey) =>
        _uiElementGetter(elementKey);
}
