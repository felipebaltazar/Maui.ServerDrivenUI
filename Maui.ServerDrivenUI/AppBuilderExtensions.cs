using EasyCaching.LiteDB;
using Maui.ServerDrivenUI.Models;
using Maui.ServerDrivenUI.Services;
using Microsoft.Extensions.Logging;
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

        builder.Services.AddEasyCaching(option =>
        {
            option.UseLiteDB(config =>
            {
                var dbFilePath = FileSystem.Current.AppDataDirectory + "sduicache.ldb";

                config.DBConfig = new LiteDBDBOptions
                {
                    FileName = dbFilePath,
                    ConnectionType = LiteDB.ConnectionType.Shared
                };
            });
        });

        builder.ConfigureLifecycleEvents(b =>
        {
#if IOS
            b.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => 
            {
                var service = Application.Current!.Handler.MauiContext!.Services.GetService<IServerDrivenUIService>();
                _ = Task.Run(service!.FetchAsync);
                return false;
            }));
#elif ANDROID
            b.AddAndroid(android => android.OnCreate(async (activity, bundle) =>
            {
                var service = Application.Current!.Handler.MauiContext!.Services.GetService<IServerDrivenUIService>();
                await service!.FetchAsync().ConfigureAwait(false);
            }));
#else
            throw new NotImplementedException("Platform not implemented");
#endif
        });

        return builder;
    }
}
