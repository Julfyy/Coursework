using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemLibrary;

namespace PawnshopNamespace
{
    public class Pawnshop
    {
        public decimal Budget;
        private Dictionary<string, PawnItem> _itemsList;

        private class PawnItem : Item
        {
            public bool IsAvailableForSell
            {
                get
                {
                    if (DateTime.Now.CompareTo(DateOfReturning) < 0)
                    {
                        return true;
                    }

                    return false;
                }
            }

            internal readonly DateTime DateOfReturning;
            internal readonly Client ClientRef;
            internal readonly double InterestRate;

            internal PawnItem(Item item, ref Client client, TimeSpan loanPeriod)
                : base(item)
            {
                InterestRate = loanPeriod.TotalHours * 0.05; //Зростаючий процент боргу залежно від строку
                ClientRef = client;
                DateOfReturning = DateTime.Now.Add(loanPeriod);
            }
        }

        //Method returns true, if adding was successful
        public bool AddItem(Item item, ref Client client, TimeSpan loanPeriod)
        {
            //Checking input data
            if (item.Equals(null))
            {
                return false; //TODO Exceptions
            }

            if (client.Name.Length < 1)
            {
                return false;
            }

            if (Budget > item.Value)
            {
                Budget -= item.Value;
            }

            _itemsList.Add(item.Name, new PawnItem(item, ref client, loanPeriod));

            //TODO Exception for existing keys

            client.Budget += item.Value;

            return true;
        }

        public bool BuyItem(string itemName, ref Client client)
        {
            if (_itemsList.ContainsKey(itemName))
            {
                var item = _itemsList[itemName];
                
                if (client.Equals(item.ClientRef)) // Якщо це той самий клієнт
                {
                    if (item.IsAvailableForSell) //Пізніше сроку
                    {
                        var increasedValue = item.Value + item.Value * (decimal) item.InterestRate;
                        client.Budget -= increasedValue;
                        Budget += increasedValue;
                        return true;
                    } 
                    client.Budget -= item.Value;
                    Budget += item.Value;
                    _itemsList.Remove(itemName);
                    return true;
                }
                
                //Якщо це інший клієнт
                if (item.IsAvailableForSell) //Пізніше срокуу
                {
                    client.Budget -= item.Value;
                    Budget += item.Value;
                    _itemsList.Remove(itemName);
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            string info = $"{"Pawnshop Info", 20} {DateTime.Now}\n";
            info += $"{"ItemName", -15} {"Value", -10} {"ClientName", -15} {"IsAvailable", -15}\n";
            foreach (var item in _itemsList)
            {
                info += $"{item.Key,-15} {item.Value.Value, -10} {item.Value.ClientRef.Name, -15} {item.Value.IsAvailableForSell, -15}\n";
            }

            return info;
        }

        //Basic constructor
        public Pawnshop(decimal budget)
        {
            Budget = budget;
            _itemsList = new Dictionary<string, PawnItem>();
        }
    }
}