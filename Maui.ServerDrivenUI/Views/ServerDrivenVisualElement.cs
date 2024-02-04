using Maui.ServerDrivenUI.Services;

namespace Maui.ServerDrivenUI.Views;

internal class ServerDrivenVisualElement
{
    internal static async Task InitializeComponentAsync(IServerDrivenVisualElement element)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() => element.State = UIElementState.Loading);

            var serverDrivenUiService = ServiceProviderHelper.ServiceProvider?.GetService<IServerDrivenUIService>();
            if (serverDrivenUiService != null)
            {
                var xaml = await serverDrivenUiService.GetXamlAsync(element.ServerKey ?? element.GetType().Name).ConfigureAwait(false);
                MainThread.BeginInvokeOnMainThread(() => {
                    var onLoaded = element.OnLoaded;
                    (element as VisualElement)?.LoadFromXaml(xaml);

                    if (element is ServerDrivenContentPage page)
                    {
                        if (page.Content is null)
                        {
                            _ = Task.Run(() => InitializeComponentAsync(element));
                            return;
                        }
                    }
                    else if (element is ServerDrivenView view)
                    {
                        if (view.Content is null)
                        {
                            _ = Task.Run(() => InitializeComponentAsync(element));
                            return;
                        }
                    }

                    element.OnLoaded = onLoaded;
                    element.State = UIElementState.Loaded;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    element.State = UIElementState.Error;
                    element.OnError(new DependencyRegistrationException("IServerDrivenUIService not found, make sure you are calling 'ConfigureServerDrivenUI(s=> s.RegisterElementGetter((k)=> yourApiCall(k)))'"));
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

    internal static void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not IServerDrivenVisualElement view
            || newValue is not UIElementState newState)
            return;

        view.OnStateChanged(newState);
        if(newState is UIElementState.Loaded)
            view.OnLoaded?.Invoke();
    }
}
