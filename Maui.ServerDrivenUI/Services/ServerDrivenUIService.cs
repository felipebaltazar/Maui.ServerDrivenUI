using EasyCaching.Core;

namespace Maui.ServerDrivenUI.Services;

public class ServerDrivenUIService
    (IServerDrivenUISettings settings, IEasyCachingProvider cacheProvider) : IServerDrivenUIService
{
    #region Fields

    private readonly IEasyCachingProvider _cacheProvider = cacheProvider;
    private readonly IServerDrivenUISettings _settings = settings;

    private readonly TaskCompletionSource<bool> _fetchFinished = new();

    private ServerUIElement[] _memoryCache = [];

    #endregion

    #region IServerDrivenUIService

    public Task ClearCacheAsync() =>
        _cacheProvider.RemoveAllAsync(_settings.CacheEntryKeys);

    public async Task FetchAsync()
    {
        _memoryCache = await GetElementsAsync().ConfigureAwait(false);

        try
        {
            await SaveIntoCacheAsync(_memoryCache).ConfigureAwait(false);
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
        var xaml = await _cacheProvider.GetAsync<string>(elementKey)
                                          .ConfigureAwait(false);

        // If the value is not in cache wait fetch finish, then try to get it from the cache again
        if (!(xaml?.HasValue ?? false) && await _fetchFinished.Task.ConfigureAwait(false))
        {
            xaml = await _cacheProvider.GetAsync<string>(elementKey).ConfigureAwait(false)
                ?? throw new KeyNotFoundException($"Visual element not found for specified key: '{elementKey}'");
        }

        var xamlValue = xaml?.Value;
        if (!(xaml?.HasValue ?? false))
            xamlValue = _memoryCache.FirstOrDefault(e => e.Key == elementKey)?.ToXaml();

        return xamlValue
            ?? string.Empty;
    }

    #endregion

    #region Private Methods

    private Task SaveIntoCacheAsync(ServerUIElement[] elements) =>
        Task.WhenAll(elements.Select(e => _cacheProvider.SetAsync(e.Key, e.ToXaml(), _settings.UIElementCacheExpiration)));

    private Task<ServerUIElement[]> DownloadServerElementsAsync()
    {
        if (_settings.ElementResolver is null)
            throw new DependencyRegistrationException("You need to set 'ElementGetter' in 'ConfigureServerDrivenUI(s=> s.RegisterElementGetter((k)=> yourApiCall(k)))' registration.");

        return Task.WhenAll(_settings.CacheEntryKeys.Select(async k => {
            var element = await _settings.ElementResolver.GetElementAsync(k).ConfigureAwait(false);
            element.Key = k;
            return element;
        }));
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
