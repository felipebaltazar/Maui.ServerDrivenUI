using System.Text.Json;

namespace Maui.ServerDrivenUI.Sample;

public class MyApi
{
    private static readonly HttpClient _httpClient = new HttpClient() {
        BaseAddress = new Uri("https://serverdrivenui.azurewebsites.net/")
    };

    private static readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<ServerUIElement> GetElement(string key)
    {
        var response = await _httpClient.GetAsync($"ServerDrivenUI?key={key}").ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            var contentStream = response.Content.ReadAsStream();
            var result = await JsonSerializer.DeserializeAsync<ServerUIElement>(contentStream, _options);
            if (result != null)
                return result;
        }

        throw new Exception("Fake api error");
    }
}
