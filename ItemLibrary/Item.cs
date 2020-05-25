using System;

namespace ItemLibrary
{
    public class Item
    {
        public readonly String Name;
        public readonly decimal Value;
        public readonly Categories Category;

        public Item(String name, decimal value, Categories category)
        {
            Name = name;
            Value = value;
            Category = category;
        }

        protected Item(Item item)
        {
            Name = item.Name;
            Value = item.Value;
            Category = item.Category;
        }

    }
}