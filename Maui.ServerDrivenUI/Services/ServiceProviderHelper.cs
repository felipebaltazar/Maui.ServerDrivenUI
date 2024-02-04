namespace Maui.ServerDrivenUI.Services;

internal static class ServiceProviderHelper
{
    public static IServiceProvider? ServiceProvider =>
#if WINDOWS
        MauiWinUIApplication.Current?.Services;
#else
        IPlatformApplication.Current?.Services;
#endif
}
