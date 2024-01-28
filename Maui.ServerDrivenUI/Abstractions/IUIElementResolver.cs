namespace Maui.ServerDrivenUI;

public interface IUIElementResolver
{
    Task<ServerUIElement> GetElementAsync(string elementKey);
}
