using System;
using System.Collections.Generic;
using PocketCqrs;

namespace ting.Domain
{
    public class Inventory : EventSourcedAggregate
    {
        public Inventory(IEnumerable<IEvent> events) : base(events)
        {
        }

        public IList<Item> Items { get; set; }

        public void AddNewItem(string name, string fileName)
        {
            var @event = new NewItemWasCreated(Guid.NewGuid(), name, fileName);
            Append(@event);
        }

        public void When(NewItemWasCreated e)
        {
            if (Items is null) Items = new List<Item>();
            Items.Add(new Item { ItemId = e.ItemId, Name = e.Name, ImageFilename = e.ImageFilename, Location = string.Empty });
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
        public string InventoryId { get; set; }

        public InventoryWasInitialized(string id)
        {
            InventoryId = id;
        }
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
}