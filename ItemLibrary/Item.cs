using System;

namespace ItemLibrary
{
    public class Item
    {
        public String Name;
        public decimal Value;
        private Categories _category;

        public Item(String name, decimal value, Categories category)
        {
            Name = name;
            Value = value;
            _category = category;
        }

        public Item(Item item)
        {
            Name = item.Name;
            Value = item.Value;
            _category = item._category;
        }

    }
}