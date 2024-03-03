namespace Maui.ServerDrivenUI;

public interface IServerDrivenVisualElement
{
    string? ServerKey { get; set; }

    UIElementState State { get; set; }

    Action? OnLoaded { get; set; }

    DataTemplate LoadingTemplate { get; set; }

    DataTemplate ErrorTemplate { get; set; }

    void OnStateChanged(UIElementState newState);

    void OnError(Exception ex);
}
