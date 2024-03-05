# Maui.ServerDrivenUI
Server Driven UI library for dotnet MAUI. New features to be deployed on all platforms simultaneously via a backend change, without releasing new versions of the native apps.


 [![NuGet](https://img.shields.io/nuget/v/ServerDrivenUI.Maui.svg)](https://www.nuget.org/packages/ServerDrivenUI.Maui/)
 
 [![Build and publish packages](https://github.com/felipebaltazar/Maui.ServerDrivenUI/actions/workflows/PackageCI.yml/badge.svg)](https://github.com/felipebaltazar/Maui.ServerDrivenUI/actions/workflows/PackageCI.yml)


 Sample Api Response: https://serverdrivenui.azurewebsites.net/ServerDrivenUI?key=MyView

 ## Getting started

- Install the ServerDrivenUI.Maui package

 ```
 Install-Package ServerDrivenUI.Maui -Version 8.0.31-pre
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
                s.RegisterElementGetter((key, provider) => provider.GetService<IYourApiService>().YourApiMethod(key)); //Register which api will be used to get the UI

                s.AddServerElement("MyView"); // Register the visual elements that will follow server driven ui
            });

		return builder.Build();
	}
}
```

- You can now use the ServerDrivenUI Elements, defining the key that will be used to get the UI from the API
- You can also define a LoadingTemplate and an ErrorTemplate to be shown while the UI is being fetched from the API


```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    x:Class="Maui.ServerDrivenUI.Sample.MainPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <ServerDrivenView x:Name="sduiView" ServerKey="MyView">
        <ServerDrivenView.ErrorTemplate>
            <DataTemplate>
                <StackLayout>
                    <Label Text="Unexpected error" />
                </StackLayout>
            </DataTemplate>
        </ServerDrivenView.ErrorTemplate>

        <ServerDrivenView.LoadingTemplate>
            <DataTemplate>
                <StackLayout>
                    <Label Text="Loading..." />
                </StackLayout>
            </DataTemplate>
        </ServerDrivenView.LoadingTemplate>
    </ServerDrivenView>

</ContentPage>

```

## Know issues

[MAUI 16809 - Label().LoadFromXaml does not work when using Label.FormattedText](https://github.com/dotnet/maui/issues/16809)

We are currently doing a [workaround](https://github.com/felipebaltazar/Maui.ServerDrivenUI/blob/main/Maui.ServerDrivenUI/Services/XamlConverterService.cs#L119) to make FormattedText work with ServerDrivenUI

## Repo Activity

![Alt](https://repobeats.axiom.co/api/embed/e3457a9dc9131c33ca38ceb2203bfffa67864080.svg "Repo activity analytics image")
