using System.Windows.Input;

namespace Maui.ServerDrivenUI.Sample
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public MainPage()
        {
            InitializeComponent();
            sduiView.OnLoaded = OnSDUILoaded;
        }

        public void OnSDUILoaded()
        {
            if(sduiView.FindByName("myButton") is Button btn)
            {
                btn.Clicked += (s, e) => {
                    count++;
                    btn.Text = $"Cliked {count} times!";
                };
            }
        }
    }
}
