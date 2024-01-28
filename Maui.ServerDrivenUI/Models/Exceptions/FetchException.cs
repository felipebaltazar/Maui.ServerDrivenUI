namespace Maui.ServerDrivenUI;

public sealed class FetchException : Exception
{
    public FetchException(string message) : base(message)
    {
    }

    public FetchException(string message, Exception inner) : base(message, inner) 
    {
    }
}
