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

    public static readonly BindableProperty LoadingTemplateProperty = BindableProperty.Create(
            nameof(LoadingTemplate),
            typeof(DataTemplate),
            typeof(ServerDrivenView),
            null);

    public static readonly BindableProperty ErrorTemplateProperty = BindableProperty.Create(
        nameof(ErrorTemplate),
        typeof(DataTemplate),
        typeof(ServerDrivenView),
        null);

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

    public DataTemplate LoadingTemplate
    {
        get => (DataTemplate)GetValue(LoadingTemplateProperty);
        set => SetValue(LoadingTemplateProperty, value);
    }

    public DataTemplate ErrorTemplate
    {
        get => (DataTemplate)GetValue(ErrorTemplateProperty);
        set => SetValue(ErrorTemplateProperty, value);
    }

    public Action? OnLoaded
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
