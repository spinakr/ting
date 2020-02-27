using System;
using System.Collections.Generic;
using PocketCqrs;

namespace ting.Domain
{
    public class Item : EventSourcedAggregate
    {
        public Item() { }

        public Item(IEnumerable<IEvent> events) : base(events) { }

        public string Name { get; set; }
        public string Location { get; set; }

        public static Item NewItem(string name)
        {
            var @event = new NewItemWasCreated(Guid.NewGuid().ToString(), name);
            var newAccount = new Item();
            newAccount.Append(@event);
            return newAccount;
        }

        public void When(NewItemWasCreated e)
        {
            Id = e.ItemId;
            Name = e.Name;
            Location = string.Empty;
        }
    }

    public class NewItemWasCreated : IEvent
    {
        public string ItemId { get; set; }
        public string Name { get; set; }

        public NewItemWasCreated(string id, string name)
        {
            ItemId = id;
            Name = name;
        }
    }
}