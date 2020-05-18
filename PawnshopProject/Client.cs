using System;

namespace PawnshopNamespace
{
    public class Client
    {
        public String Name;
        public decimal _budget;

        public Client(String name, decimal budget)
        {
            Name = name;
            _budget = budget;
        }
    }
}