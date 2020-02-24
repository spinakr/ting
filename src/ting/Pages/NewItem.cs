using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Ting.Pages
{
    [BindProperties]
    public class NewItem : PageModel
    {
        private readonly ILogger<NewItem> _logger;

        public NewItem(ILogger<NewItem> logger)
        {
            _logger = logger;
        }

        public IFormFile Upload { get; set; }
        public string Name { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            var rootDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var file = Path.Combine(Environment.GetEnvironmentVariable("contentPath") ?? rootDir, "images", Name);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }
        }
    }
}
