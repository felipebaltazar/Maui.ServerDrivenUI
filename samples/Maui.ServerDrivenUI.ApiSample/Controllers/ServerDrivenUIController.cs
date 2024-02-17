using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace Maui.ServerDrivenUI.ApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerDrivenUIController : ControllerBase
    {
        private readonly ILogger<ServerDrivenUIController> _logger;

        public ServerDrivenUIController(ILogger<ServerDrivenUIController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUIElement")]
        public async Task<ServerUIElement> Get(string key)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{key}.json";
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
                throw new Exception("Resource not found");

            var result = await JsonSerializer.DeserializeAsync<ServerUIElement>(stream) ?? throw new Exception("Fake api error");

            return result;
        }
    }
}
