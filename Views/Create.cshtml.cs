using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentApi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    [BindProperty]
    public Student Student { get; set; }

    public CreateModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("https://localhost:5001/api/Students", Student);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }
}