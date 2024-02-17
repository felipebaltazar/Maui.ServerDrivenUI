namespace Maui.ServerDrivenUI.Sample;

[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }
}
