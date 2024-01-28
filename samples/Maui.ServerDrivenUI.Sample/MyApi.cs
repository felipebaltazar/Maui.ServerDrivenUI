using System.Text.Json;
using System.Text.Json.Serialization;

namespace Maui.ServerDrivenUI.Sample;

public class MyApi
{
    public static async Task<ServerUIElement> GetElement(string key)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync($"{key}.json");
        var result = await JsonSerializer.DeserializeAsync<ServerUIElement>(stream) ?? throw new Exception("Fake api error");

        return result;
    }
}
