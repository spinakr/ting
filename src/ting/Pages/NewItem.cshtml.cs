using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PocketCqrs;
using PocketCqrs.EventStore;
using Ting.Domain;

namespace Ting.Pages
{
    [BindProperties]
    public class NewItem : PageModel
    {
        private readonly ILogger<NewItem> _logger;
        private readonly IMessaging _messaging;

        public NewItem(IMessaging messaging, ILogger<NewItem> logger)
        {
            _logger = logger;
            _messaging = messaging;
        }

        public IFormFile Upload { get; set; }
        public string Name { get; set; }

        public void OnGet()
        {
        }

        public async Task<RedirectToPageResult> OnPostAsync()
        {
            _messaging.Dispatch(new AddNewItemCommand
            {
                ImageFile = Upload,
                ItemName = Name
            });
            return RedirectToPage("NewItem");
        }
    }

    public class AddNewItemCommand : ICommand
    {
        public string ItemName { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class AddNewItemCommandHandler : ICommandHandler<AddNewItemCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IEventStore _evnetStore;
        private readonly ILogger<AddNewItemCommandHandler> _logger;

        public AddNewItemCommandHandler(IConfiguration configuration, IEventStore eventStore, ILogger<AddNewItemCommandHandler> logger)
        {
            _evnetStore = eventStore;
            _configuration = configuration;
            _logger = logger;
        }


        public Result Handle(AddNewItemCommand cmd)
        {
            var fileName = string.Empty;
            if (cmd.ImageFile is object) fileName = SaveFileToDisk(cmd);

            var eventStream = _evnetStore.LoadEventStream("items");
            var inventory = new Inventory(eventStream.Events);
            inventory.AddNewItem(cmd.ItemName, $"images/{fileName}");

            _evnetStore.AppendToStream("items", inventory.PendingEvents, eventStream.Version);
            return Result.Complete();
        }

        private string SaveFileToDisk(AddNewItemCommand cmd)
        {
            var filePostfix = cmd.ImageFile.FileName.Split('.').Last();
            var fileStorageLocation = $"{_configuration.GetValue<string>("FILE_BASE_PATH") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/ting/images";
            Directory.CreateDirectory(fileStorageLocation);
            var fileName = $"{cmd.ItemName}.{filePostfix}";
            var fileNameWithLocation = Path.Combine(fileStorageLocation, fileName);
            _logger.LogInformation($"Storing file to locaiton {fileNameWithLocation}");
            using (var fileStream = new FileStream(fileNameWithLocation, FileMode.Create))
            {
                cmd.ImageFile.CopyToAsync(fileStream).GetAwaiter().GetResult();
            }

            return fileName;
        }
    }
}
