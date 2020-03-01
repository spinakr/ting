using System;
using System.IO;
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
    public class ItemDetails : PageModel
    {
        private readonly ILogger<ItemDetails> _logger;
        private readonly IMessaging _messaging;

        public ItemDetails(IMessaging messaging, ILogger<ItemDetails> logger)
        {
            _logger = logger;
            _messaging = messaging;
        }

        public Domain.Item SelectedItem { get; set; }

        public void OnGet(Guid itemId)
        {
            SelectedItem = _messaging.Dispatch(new GetItemQuery { ItemId = itemId });
        }
    }

    public class GetItemQuery : IQuery<Domain.Item>
    {
        public Guid ItemId { get; set; }
    }

    public class GetItemQueryHandler : IQueryHandler<GetItemQuery, Domain.Item>
    {
        private readonly IConfiguration _configuration;
        private readonly IEventStore _evnetStore;
        private readonly ILogger<AddNewItemCommandHandler> _logger;

        public GetItemQueryHandler(IConfiguration configuration, IEventStore eventStore, ILogger<AddNewItemCommandHandler> logger)
        {
            _evnetStore = eventStore;
            _configuration = configuration;
            _logger = logger;
        }


        public Domain.Item Handle(GetItemQuery query)
        {
            var eventStream = _evnetStore.LoadEventStream("items");
            var inventory = new Inventory(eventStream.Events);

            return inventory.Items.SingleOrDefault(x => x.ItemId == query.ItemId);
        }
    }
}
