# Maui.ServerDrivenUI
Server Driven UI library for dotnet MAUI. New features to be deployed on all platforms simultaneously via a backend change, without releasing new versions of the native apps.


 [![NuGet](https://img.shields.io/nuget/v/ServerDrivenUI.Maui.svg)](https://www.nuget.org/packages/ServerDrivenUI.Maui/)
 
 [![Build and publish packages](https://github.com/felipebaltazar/Maui.ServerDrivenUI/actions/workflows/PackageCI.yml/badge.svg)](https://github.com/felipebaltazar/Maui.ServerDrivenUI/actions/workflows/PackageCI.yml)


 ## Getting started

- Install the ServerDrivenUI.Maui package

 ```
 Install-Package ServerDrivenUI.Maui -Version 1.0.17
 ```

- Add UseServerDrivenUI declaration to your `MauiAppBuilder` and configure it to connect to your API

```csharp
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
            .ConfigureServerDrivenUI(s =>
            {
                s.RegisterElementGetter(key => MyApi.GetElement(key)); //Register which api will be used to get the UI

                s.AddServerElement("MyView"); // Register the visual elements that will follow server driven ui
            });

		return builder.Build();
	}
}
```

- You can now receive the xaml and load it 

```csharp

var xaml = await _serverDrivenUIService.GetXamlAsync("MyView").ConfigureAwait(false);
MainThread.BeginInvokeOnMainThread(() =>
{
    sduiView.LoadFromXaml(xaml);
});
```