using Maui.ServerDrivenUI.Services;

namespace Maui.ServerDrivenUI.Views;

internal class ServerDrivenVisualElement
{
    private const string SERVICE_NOT_FOUND = "IServerDrivenUIService not found, make sure you are calling 'ConfigureServerDrivenUI(s=> s.RegisterElementGetter((k)=> yourApiCall(k)))'";

    internal static async Task InitializeComponentAsync(IServerDrivenVisualElement element)
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

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var onLoaded = element.OnLoaded;
                    var visualElement = (element as VisualElement);
                    var currentBindingContext = visualElement?.BindingContext;

                    visualElement?.LoadFromXaml(xaml);

                    if (!IsXamlLoaded(element))
                        return;

                    if(visualElement != null)
                        visualElement.BindingContext = currentBindingContext;

                    element.OnLoaded = onLoaded;
                    element.State = UIElementState.Loaded;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    element.State = UIElementState.Error;
                    element.OnError(new DependencyRegistrationException(SERVICE_NOT_FOUND));
                });
            }
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                element.State = UIElementState.Error;
                element.OnError(ex);
            });
        }
    }

    private static bool IsXamlLoaded(IServerDrivenVisualElement element)
    {
        switch (element)
        {
            case ServerDrivenContentPage page when page.Content is null:
                _ = Task.Run(() => InitializeComponentAsync(element));
                return false;
            case ServerDrivenView view when view.Content is null:
                _ = Task.Run(() => InitializeComponentAsync(element));
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
