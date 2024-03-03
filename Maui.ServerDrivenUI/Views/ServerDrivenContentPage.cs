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

    public Action? OnLoaded
    {
        get;
        set;
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
