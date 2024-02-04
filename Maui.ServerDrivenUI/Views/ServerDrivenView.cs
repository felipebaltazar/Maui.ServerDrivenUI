using Maui.ServerDrivenUI.Views;

namespace Maui.ServerDrivenUI;

public class ServerDrivenView : ContentView, IServerDrivenVisualElement
{
    #region BindableProperties

    public static readonly BindableProperty ServerKeyProperty = BindableProperty.Create(
            nameof(ServerKey),
            typeof(string),
            typeof(ServerDrivenView),
            null);

    public static readonly BindableProperty StateProperty = BindableProperty.Create(
            nameof(State),
            typeof(UIElementState),
            typeof(ServerDrivenView),
            UIElementState.None,
            propertyChanged: ServerDrivenVisualElement.OnStatePropertyChanged);

    #endregion

    #region Properties

    public string? ServerKey
    {
        get => (string?)GetValue(ServerKeyProperty);
        set => SetValue(ServerKeyProperty, value);
    }

    public UIElementState State
    {
        get => (UIElementState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public Action OnLoaded
    {
        get;
        set;
    }

    #endregion

    #region Constructors

    public ServerDrivenView() =>
    _ = Task.Run(InitializeComponentAsync);

    #endregion

    #region IServerDrivenVisualElement

    protected virtual Task InitializeComponentAsync() =>
        ServerDrivenVisualElement.InitializeComponentAsync(this);

    public virtual void OnStateChanged(UIElementState newState)
    {
    }

    public virtual void OnError(Exception ex)
    {
    }

    #endregion
}
