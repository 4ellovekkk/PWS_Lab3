using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentApi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    [BindProperty]
    public Student? Student { get; set; }

    public DeleteModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _clientFactory.CreateClient();
        Student = await client.GetFromJsonAsync<Student>($"https://localhost:5001/api/Students/{id}");

        if (Student == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.DeleteAsync($"https://localhost:5001/api/Students/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }
}