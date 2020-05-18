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
            public bool IsAvailableToBuy;
            internal readonly DateTime dateOfReturning;
            internal Client ClientRef;
            internal double InterestRate;
            
            protected internal PawnItem(Item item, ref Client client, TimeSpan loanPeriod) 
                : base(item)
            {
                InterestRate = loanPeriod.TotalHours * 0.05 ; //Зростаючий процент боргу залежно від строку
                ClientRef = client;
                IsAvailableToBuy = true;
                dateOfReturning = DateTime.Now.Add(loanPeriod);
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
            if (client.Name.Length < 1) {
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
                if (client.Equals(item.ClientRef)) //Якщо це той самий клієнт
                {
                    if (DateTime.Now.CompareTo(item.dateOfReturning) < 0)
                    {
                        var increasedValue = item.Value + item.Value * (decimal) item.InterestRate;
                        client.Budget -= increasedValue;
                        Budget += increasedValue;
                    }
                }
                else
                {
                    client.Budget -= item.Value;
                    Budget += item.Value;
                    _itemsList.Remove(itemName);
                    return true;
                }
            }
            return false;
        }
        

        //Basic constructor
        public Pawnshop(decimal budget)
        {
            Budget = budget;
            _itemsList = new Dictionary<string, PawnItem>();
        }
        
    }
}