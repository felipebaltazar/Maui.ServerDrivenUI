using EasyCaching.Core;

namespace Maui.ServerDrivenUI.Services;

public class ServerDrivenUIService
    (IServerDrivenUISettings settings, IEasyCachingProvider cacheProvider) : IServerDrivenUIService
{
    #region Fields

    private readonly IEasyCachingProvider _cacheProvider = cacheProvider;
    private readonly IServerDrivenUISettings _settings = settings;

    private readonly TaskCompletionSource<bool> _fetchFinished = new();

    #endregion

    #region IServerDrivenUIService

    public Task ClearCacheAsync() =>
        _cacheProvider.RemoveAllAsync(_settings.CacheEntryKeys);

    public async Task FetchAsync()
    {
        var elements = await GetElementsAsync().ConfigureAwait(false);

        try
        {
            await SaveIntoCacheAsync(elements).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _fetchFinished.TrySetResult(false);
            throw new FetchException("Error while saving server elements into cache", ex);
        }

        _fetchFinished.TrySetResult(true);
    }

    public async Task<string> GetXamlAsync(string elementKey)
    {
        // Try to get the value from cache
        var element = await _cacheProvider.GetAsync<ServerUIElement>(elementKey)
                                          .ConfigureAwait(false);

        // If the value is not in cache wait fetch finish, then try to get it from the cache again
        if (!(element?.HasValue ?? true) && await _fetchFinished.Task.ConfigureAwait(false))
        {
            element = await _cacheProvider.GetAsync<ServerUIElement>(elementKey).ConfigureAwait(false)
                ?? throw new KeyNotFoundException($"Visual element not found for specified key: '{elementKey}'");

        }

        return element?.Value?.ToXaml()
            ?? string.Empty;
    }

    #endregion

    #region Private Methods

    private Task SaveIntoCacheAsync(ServerUIElement[] elements) =>
        Task.WhenAll(elements.Select(e => _cacheProvider.SetAsync(e.Key, e, _settings.UIElementCacheExpiration)));

    private Task<ServerUIElement[]> DownloadServerElementsAsync()
    {
        if (_settings.ElementResolver is null)
            throw new DependencyRegistrationException("You need to set 'ElementGetter' in 'ConfigureServerDrivenUI(s=> s.RegisterElementGetter((k)=> yourApiCall(k)))' registration.");

        return Task.WhenAll(_settings.CacheEntryKeys.Select(_settings.ElementResolver.GetElementAsync));
    }

    private async Task<ServerUIElement[]> GetElementsAsync()
    {
        try
        {
            return await DownloadServerElementsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _fetchFinished.TrySetResult(false);
            throw new FetchException("Error while downloading server elements", ex);
        }
    }

    #endregion
}
