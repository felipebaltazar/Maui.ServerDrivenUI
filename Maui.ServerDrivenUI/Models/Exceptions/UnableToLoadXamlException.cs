namespace Maui.ServerDrivenUI.Models.Exceptions;

public class UnableToLoadXamlException : Exception
{
    public string Xaml
    {
        get;
    }

    public UnableToLoadXamlException(string message, string xaml, Exception inner) : base(message, inner)
    {
        Xaml = xaml;
    }
}
