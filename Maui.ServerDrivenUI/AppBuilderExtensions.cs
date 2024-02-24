using EasyCaching.LiteDB;
using Maui.ServerDrivenUI.Models;
using Maui.ServerDrivenUI.Services;
using Microsoft.Maui.LifecycleEvents;

namespace Maui.ServerDrivenUI;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder ConfigureServerDrivenUI(this MauiAppBuilder builder, Action<IServerDrivenUISettings> configure)
    {
        var settings = new ServerDrivenUISettings();
        configure(settings);

        builder.Services.AddSingleton<IServerDrivenUISettings>(settings);
        builder.Services.AddSingleton<IServerDrivenUIService, ServerDrivenUIService>();

        builder.Services.AddEasyCaching(o =>
            o.UseLiteDB(c => ConfigureDb(c, settings)));

        builder.ConfigureLifecycleEvents(b => {
#if IOS
            b.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => InitServerDrivenUIService()));
#elif ANDROID
            b.AddAndroid(android => android.OnCreate((activity, bundle) => InitServerDrivenUIService()));
#elif MACCATALYST
            b.AddiOS(macCatalyst => macCatalyst.FinishedLaunching((app, launchOptions) => InitServerDrivenUIService()));
#elif WINDOWS
            b.AddWindows(windows => windows.OnLaunched((app, args) => InitServerDrivenUIService()));
#else
            throw new NotImplementedException("Platform not implemented");
#endif
        });

        return builder;
    }

    private static void ConfigureDb(LiteDBOptions config, ServerDrivenUISettings settings)
    {
        var dbFilePath = settings.CacheFilePath
            ?? Path.Combine(FileSystem.Current.AppDataDirectory, "sduicache.ldb");

        config.DBConfig = new LiteDBDBOptions {
            FileName = dbFilePath,
            ConnectionType = LiteDB.ConnectionType.Shared,
        };
    }

    private static bool InitServerDrivenUIService()
    {
        var service = ServiceProviderHelper.ServiceProvider!.GetService<IServerDrivenUIService>();
        _ = Task.Run(service!.FetchAsync);

        return false;
    }
}
