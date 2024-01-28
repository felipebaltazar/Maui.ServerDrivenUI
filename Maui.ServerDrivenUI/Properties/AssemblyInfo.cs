using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

//Custom xaml schema <see href="https://docs.microsoft.com/pt-br/xamarin/xamarin-forms/xaml/custom-namespace-schemas#defining-a-custom-namespace-schema"/>
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Maui.ServerDrivenUI")]
[assembly: XmlnsDefinition("http://neocontrols.com/schemas/xaml", "Maui.ServerDrivenUI")]

//Recommended prefix <see href="https://docs.microsoft.com/pt-br/xamarin/xamarin-forms/xaml/custom-prefix"/>
[assembly: XmlnsPrefix("http://maui.serverdrivenui.com/schemas/xaml", "sdui")]

//Xaml compilation <see href="https://docs.microsoft.com/pt-br/xamarin/xamarin-forms/xaml/xamlc"/>
[assembly: XamlCompilation(XamlCompilationOptions.Skip)]