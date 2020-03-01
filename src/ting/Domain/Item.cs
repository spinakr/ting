using System;
using System.Collections.Generic;
using System.Linq;
using PocketCqrs;

namespace Ting.Domain
{
    public class Inventory : EventSourcedAggregate
    {
        public Inventory(IEnumerable<IEvent> events) : base(events)
        {
        }

        public IList<Item> Items { get; set; }

        internal void Init()
        {
            var @event1 = new InventoryWasInitialized();
            Append(@event1);
        }

        public void AddNewItem(string name, string fileName)
        {
            var @event1 = new InventoryWasInitialized();
            Append(@event1);
            var @event = new NewItemWasCreated(Guid.NewGuid(), name, fileName);
            Append(@event);
        }

        internal void UpdateItemLocation(Guid itemId, string newLocation)
        {
            var @event = new ItemLocationWasUpdated(itemId, newLocation);
            Append(@event);
        }

        public void When(InventoryWasInitialized e)
        {
            if (Items is null) Items = new List<Item>();
        }

        public void When(NewItemWasCreated e)
        {
            Items.Add(new Item { ItemId = e.ItemId, Name = e.Name, ImageFilename = e.ImageFilename, Location = string.Empty });
        }

        public void When(ItemLocationWasUpdated e)
        {
            Items.Single(x => x.ItemId == e.ItemId).Location = e.NewLocation;
        }
    }

    public class Item
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ImageFilename { get; set; }
    }
    public class InventoryWasInitialized : IEvent
    {
    }

    public class NewItemWasCreated : IEvent
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string ImageFilename { get; set; }

        public NewItemWasCreated(Guid id, string name, string imageUri)
        {
            ItemId = id;
            Name = name;
            ImageFilename = imageUri;
        }
    }

    public class ItemLocationWasUpdated : IEvent
    {
        public ItemLocationWasUpdated(Guid itemId, string newLocation)
        {
            ItemId = itemId;
            NewLocation = newLocation;
        }

        public Guid ItemId { get; set; }
        public string NewLocation { get; set; }
    }
}