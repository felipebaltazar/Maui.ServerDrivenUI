using System.Windows.Input;

namespace Maui.ServerDrivenUI.Sample
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class MainPage : ContentPage
    {
        private readonly IServerDrivenUIService _serverDrivenUIService;
        int count = 0;

        public MainPage(IServerDrivenUIService serverDrivenUIService)
        {
            _serverDrivenUIService = serverDrivenUIService;
            InitializeComponent();

            _ = Task.Run(InitializeComponentAsync);
        }

        private async Task InitializeComponentAsync()
        {
            var xaml = await _serverDrivenUIService.GetXamlAsync("MyView").ConfigureAwait(false);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                sduiView.LoadFromXaml(xaml);
            });
        }
    }
}
