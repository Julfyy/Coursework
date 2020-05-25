using System;

namespace ItemLibrary
{
    public abstract class Item
    {
        public readonly String Name;
        public readonly decimal Value;
        public readonly Categories Category;

        protected Item(String name, decimal value, Categories category)
        {
            Name = name;
            Value = value;
            Category = category;
        }
    }
}