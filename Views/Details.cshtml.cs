using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentApi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public Student Student { get; set; }

    public DetailsModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task OnGetAsync(int id)
    {
        var client = _clientFactory.CreateClient();
        Student = await client.GetFromJsonAsync<Student>($"https://localhost:5001/api/Students/{id}");
    }
}