using Maui.ServerDrivenUI.Models.Exceptions;
using Maui.ServerDrivenUI.Services;

namespace Maui.ServerDrivenUI.Views;

internal class ServerDrivenVisualElement
{
    private const string SERVICE_NOT_FOUND = "IServerDrivenUIService not found, make sure you are calling 'ConfigureServerDrivenUI(s=> s.RegisterElementGetter((k)=> yourApiCall(k)))'";
    private const string XAML_LOAD_ERROR_MESSAGE = "Error loading XAML";
    private const int MAX_RETRIES = 3;

    internal static async Task InitializeComponentAsync(IServerDrivenVisualElement element, int attempt = 0)
    {
        var errorMessage = string.Empty;
        try
        {
            ShowLoadingView(element);
            MainThread.BeginInvokeOnMainThread(() => element.State = UIElementState.Loading);

            var serverDrivenUiService = ServiceProviderHelper
                .ServiceProvider?
                .GetService<IServerDrivenUIService>();

            if (serverDrivenUiService != null)
            {
                var xaml = await serverDrivenUiService
                    .GetXamlAsync(element.ServerKey ?? element.GetType().Name)
                    .ConfigureAwait(false);

                MainThread.BeginInvokeOnMainThread(() => {
                    var onLoaded = element.OnLoaded;
                    var visualElement = (element as VisualElement);
                    var currentBindingContext = visualElement?.BindingContext;
                    var forceRetry = false;

                    try
                    {
                        visualElement?.LoadFromXaml(xaml);
                        errorMessage = string.Empty;

                        if (XamlConverterService.LabelsSpans.Any())
                        {
                            foreach (var labelSpan in XamlConverterService.LabelsSpans)
                            {
                                if (visualElement?.FindByName<Label>(labelSpan.Key) is Label label)
                                {
                                    label.FormattedText = labelSpan.Value;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var xamlException = new UnableToLoadXamlException(XAML_LOAD_ERROR_MESSAGE, xaml, ex);
                        element.OnError(xamlException);
                        forceRetry = true;
                    }

                    if (!IsXamlLoaded(element, attempt, forceRetry))
                        return;

                    if (visualElement != null)
                        visualElement.BindingContext = currentBindingContext;

                    element.OnLoaded = onLoaded;
                    element.State = UIElementState.Loaded;
                });
            }
            else
            {
                errorMessage = SERVICE_NOT_FOUND;

                MainThread.BeginInvokeOnMainThread(() => {
                    element.State = UIElementState.Error;
                    element.OnError(new DependencyRegistrationException(SERVICE_NOT_FOUND));
                });
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            MainThread.BeginInvokeOnMainThread(() => {
                element.State = UIElementState.Error;
                element.OnError(ex);
            });
        }

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            ShowErrorView(element, errorMessage);
        }
    }

    private static bool IsXamlLoaded(IServerDrivenVisualElement element, int attempt, bool forceRetry = false)
    {
        if (attempt < MAX_RETRIES && forceRetry)
        {
            _ = Task.Run(() => InitializeComponentAsync(element, attempt++));
            return false;
        }

        switch (element)
        {
            case ServerDrivenContentPage page when page.Content is null:
            case ServerDrivenView view when view.Content is null:
                if (attempt < MAX_RETRIES)
                {
                    _ = Task.Run(() => InitializeComponentAsync(element, attempt++));
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => element.State = UIElementState.Error);
                }

                return false;
        }

        return true;
    }

    private static void ShowLoadingView(IServerDrivenVisualElement element)
    {
        View loadingView = (element.LoadingTemplate?.CreateContent() as View)
            ?? CreateDefaultLoadingTemplate();

        SetContent(element, loadingView);
    }

    private static void ShowErrorView(IServerDrivenVisualElement element, string errorMessage)
    {
        try
        {
            View errorView = (element.ErrorTemplate?.CreateContent() as View)
                ?? CreateDefaultErrorTemplate(errorMessage);

            SetContent(element, errorView);
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                element.State = UIElementState.Error;
                element.OnError(ex);
            });
        }
    }

    private static void SetContent(IServerDrivenVisualElement element, View template)
    {
        if (element is ContentView contentView && template != null)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                template.BindingContext = contentView.BindingContext;
                contentView.Content = template;
            });
        }
        else if (element is ContentPage contentPage && template != null)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                template.BindingContext = contentPage.BindingContext;
                contentPage.Content = template;
            });
        }
    }

    internal static void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not IServerDrivenVisualElement view
            || newValue is not UIElementState newState)
            return;

        view.OnStateChanged(newState);
        if (newState is UIElementState.Loaded)
            view.OnLoaded?.Invoke();
    }

    internal static View CreateDefaultLoadingTemplate() =>
        new Frame() {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Black,
            BorderColor = Colors.Transparent,
            Content = new ActivityIndicator() {
                IsRunning = true,
                IsEnabled = true,
                Color = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

    internal static View CreateDefaultErrorTemplate(string errorMessage) =>
        new Frame() {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Red,
            BorderColor = Colors.Transparent,
            Content = new Label {
                Text = errorMessage,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }
        };
}
