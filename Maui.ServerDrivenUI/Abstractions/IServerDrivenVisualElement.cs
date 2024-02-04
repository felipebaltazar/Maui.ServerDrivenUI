namespace Maui.ServerDrivenUI;

public interface IServerDrivenVisualElement
{
    public string? ServerKey
    {
        get; set;
    }

    public UIElementState State
    {
        get; set;
    }
    public Action OnLoaded
    {
        get;
        set;
    }

    void OnStateChanged(UIElementState newState);
    void OnError(Exception ex);
}
