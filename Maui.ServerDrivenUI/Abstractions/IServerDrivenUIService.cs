namespace Maui.ServerDrivenUI;

public interface IServerDrivenUIService
{
    Task<string> GetXamlAsync(string elementKey);
    Task ClearCacheAsync();
    Task FetchAsync();
}
