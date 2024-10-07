using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public List<Student> Students { get; set; }

    public IndexModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient();
        Students = await client.GetFromJsonAsync<List<Student>>("https://localhost:5001/api/Students");
    }
}