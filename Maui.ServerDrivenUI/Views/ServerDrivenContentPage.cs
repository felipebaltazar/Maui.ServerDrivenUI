using Maui.ServerDrivenUI.Views;

namespace Maui.ServerDrivenUI;

public abstract class ServerDrivenContentPage : ContentPage, IServerDrivenVisualElement
{
    #region BindableProperties

    public static readonly BindableProperty ServerKeyProperty = BindableProperty.Create(
            nameof(ServerKey),
            typeof(string),
            typeof(ServerDrivenContentPage),
            null);

    public static readonly BindableProperty StateProperty = BindableProperty.Create(
            nameof(State),
            typeof(UIElementState),
            typeof(ServerDrivenContentPage),
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

    public ServerDrivenContentPage() =>
    _ = Task.Run(InitializeComponentAsync);

    #endregion

    #region Protected Methods

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
