using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Maui.ServerDrivenUI.Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit()
                .ConfigureServerDrivenUI(s =>
                {
                    s.RegisterElementGetter(key => MyApi.GetElement(key));

                    s.AddServerElement("MyView");
                });

            builder.Services.AddScoped<MainPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
