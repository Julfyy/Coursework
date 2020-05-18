using System;

namespace PawnshopNamespace
{
    public class Client
    {
        public String Name;
        public decimal Budget;

        public Client(String name, decimal budget)
        {
            Name = name;
            Budget = budget;
        }
    }
}