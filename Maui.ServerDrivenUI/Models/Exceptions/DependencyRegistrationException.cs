namespace Maui.ServerDrivenUI;

public sealed class DependencyRegistrationException : Exception
{
    public DependencyRegistrationException(string message) : base(message)
    {
    }

    public DependencyRegistrationException(string message, Exception inner) : base(message, inner)
    {
    }
}
