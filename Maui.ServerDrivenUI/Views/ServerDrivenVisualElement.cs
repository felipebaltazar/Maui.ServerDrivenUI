using Maui.ServerDrivenUI.Services;

namespace Maui.ServerDrivenUI.Views;

internal class ServerDrivenVisualElement
{
    private const string SERVICE_NOT_FOUND = "IServerDrivenUIService not found, make sure you are calling 'ConfigureServerDrivenUI(s=> s.RegisterElementGetter((k)=> yourApiCall(k)))'";
    private const int MAX_RETRIES = 3;

    internal static async Task InitializeComponentAsync(IServerDrivenVisualElement element, int attempt = 0)
    {
        try
        {
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

                    try
                    {
                        visualElement?.LoadFromXaml(xaml);

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
                        element.OnError(ex);
                    }

                    if (!IsXamlLoaded(element, attempt))
                        return;

                    if (visualElement != null)
                        visualElement.BindingContext = currentBindingContext;

                    element.OnLoaded = onLoaded;
                    element.State = UIElementState.Loaded;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    element.State = UIElementState.Error;
                    element.OnError(new DependencyRegistrationException(SERVICE_NOT_FOUND));
                });
            }
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                element.State = UIElementState.Error;
                element.OnError(ex);
            });
        }
    }

    private static bool IsXamlLoaded(IServerDrivenVisualElement element, int attempt)
    {
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

    internal static void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not IServerDrivenVisualElement view
            || newValue is not UIElementState newState)
            return;

        view.OnStateChanged(newState);
        if (newState is UIElementState.Loaded)
            view.OnLoaded?.Invoke();
    }
}
