using System.Collections.Generic;
using System.Linq;
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
    public class ItemList : PageModel
    {
        private readonly ILogger<ItemList> _logger;
        private readonly IMessaging _messaging;

        public ItemList(IMessaging messaging, ILogger<ItemList> logger)
        {
            _logger = logger;
            _messaging = messaging;
        }

        public IEnumerable<Item> Items { get; set; }

        public void OnGet()
        {
            Items = _messaging.Dispatch(new GetItems());
        }
    }

    public class GetItems : IQuery<IEnumerable<Item>>
    {
    }

    public class GetItemsHandler : IQueryHandler<GetItems, IEnumerable<Item>>
    {
        private readonly IConfiguration _configuration;
        private readonly IEventStore _evnetStore;
        private readonly ILogger<AddNewItemCommandHandler> _logger;

        public GetItemsHandler(IConfiguration configuration, IEventStore eventStore, ILogger<AddNewItemCommandHandler> logger)
        {
            _evnetStore = eventStore;
            _configuration = configuration;
            _logger = logger;
        }


        public IEnumerable<Item> Handle(GetItems query)
        {
            var eventStream = _evnetStore.LoadEventStream("items");
            var inventory = new Inventory(eventStream.Events);
            if (inventory.Items is null)
            {
                inventory.Init();
                _evnetStore.AppendToStream("items", inventory.PendingEvents, eventStream.Version);
            }

            return inventory.Items;
        }
    }
}
