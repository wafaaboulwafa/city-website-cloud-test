using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CityWeb.Models;
using System.Text.Json;

namespace CityWeb.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public IList<City> Cities { get; set; } = new List<City>();

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Index page handler called");
        
        var client = _clientFactory.CreateClient("CityApi");
        try
        {
            var response = await client.GetAsync("/api/cities");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Cities = JsonSerializer.Deserialize<List<City>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<City>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cities from API");
        }
    }
}
