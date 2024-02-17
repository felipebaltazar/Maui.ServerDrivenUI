using System.Windows.Input;

namespace Maui.ServerDrivenUI.Sample;

public class MainPageViewModel : ViewModelBase
{
    private int _count = 0;
    private string _buttonText = "Cicke me";
    private string _text = "This is a binding text loaded from xaml.";

    public string Text
    {
        get => _text;
        set => Set(ref _text, value, nameof(Text));
    }

    public string ButtonText
    {
        get => _buttonText;
        set => Set(ref _buttonText, value, nameof(ButtonText));
    }

    public ICommand ButtonClickedCommand => new Command(() =>
    {
        _count++;
        ButtonText = $"Clicked {_count} times!";
    });
}
