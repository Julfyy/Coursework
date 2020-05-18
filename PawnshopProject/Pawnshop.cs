using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemLibrary;

namespace PawnshopNamespace
{
    public class Pawnshop
    {
        private decimal _budget;
        private PawnItem[] _itemsList;

        private class PawnItem : Item
        {
            public bool IsAvailableToBuy;
            internal readonly DateTime dateOfReturning;
            internal decimal LoanAmount;
            internal Client ClientRef;
            internal double InterestRate;
            
            protected internal PawnItem(Item item, ref Client client, double interestRate, TimeSpan loanPeriod) 
                : base(item)
            {
                LoanAmount = item.Value;
                ClientRef = client;
                InterestRate = interestRate;
                IsAvailableToBuy = true;
                dateOfReturning = DateTime.Now.Add(loanPeriod);
            }
        }
        
        //Method returns true, if adding was successful
        public bool AddItem(Item item, ref Client client, double interestRate, TimeSpan loanPeriod) 
        {
            //Checking input data
            if (item.Equals(null))
            {
                return false; //To do: Exceptions
            }
            if (client.Name.Length < 1) {
                return false;
            }

            if (interestRate < 0)
            {
                return false;
            }

            if (_budget > item.Value)
            {
                _budget -= item.Value;
            }
            
            //Resizing container array
            Array.Resize(ref _itemsList, _itemsList.Length + 1);
            //Adding new object of PawnItem class, with data, copied from Item object
            _itemsList[^1] = new PawnItem(item, ref client, interestRate, loanPeriod);
            
            
           
            client._budget += item.Value;
           
           
            return true;
        }

        public bool BuyItem(Item item, ref Client client)
        {
            for (int i = 0; i < _itemsList.Length; i++)
            {
                if (item.Equals(_itemsList[i]))
                {
                    _itemsList[i] = null;
                    client._budget -= _itemsList[i].LoanAmount;
                    return true;
                }
            }

            return false;
        }
        

        //Basic constructor
        public Pawnshop(decimal budget)
        {
            _budget = budget;
            _itemsList = new PawnItem[0];
        }
        
    }
}