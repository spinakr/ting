using System;
using System.Collections.Generic;
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
        private readonly IConfiguration _configuration;
        private readonly IEventStore _evnetStore;
        private readonly ILogger<ItemDetails> _logger;

        public ItemDetails(IEventStore eventStore, ILogger<ItemDetails> logger)
        {
            _logger = logger;
            _evnetStore = eventStore;
        }

        public Domain.Item SelectedItem { get; set; }
        public IList<string> PossibleLocations { get; set; }

        public void OnGet(Guid itemId)
        {
            var eventStream = _evnetStore.LoadEventStream("items");
            var inventory = new Inventory(eventStream.Events);

            SelectedItem = inventory.Items.SingleOrDefault(x => x.ItemId == itemId);
            PossibleLocations = inventory.Items.GroupBy(x => x.Location).Select(x => x.First().Location).ToList();
        }

        public void OnPost(Guid itemId, string newLocation)
        {
            var eventStream = _evnetStore.LoadEventStream("items");
            var inventory = new Inventory(eventStream.Events);
            inventory.UpdateItemLocation(itemId, newLocation);
            _evnetStore.AppendToStream("items", inventory.PendingEvents, eventStream.Version);

        }
    }
}
