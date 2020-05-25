using System;
using System.Collections;
using System.Collections.Generic;
using ItemLibrary;

namespace PawnshopNamespace
{
    public class Pawnshop
    {
        private decimal _budget;
        private List<PawnItem> _itemsList;
        private Dictionary<Categories, Queue> _queues;
        private Dictionary<String, Client> _clients;

        private class PawnItem : Item
        {
            public bool IsAvailableForSell
            {
                get
                {
                    if (DateTime.Now.CompareTo(_dateOfReturning) < 0)
                        return true;
                    return false;
                }
            }

            //private static int Id;
            private readonly DateTime _dateOfReturning;
            internal readonly Client ClientRef;
            internal readonly double InterestRate;

            internal PawnItem(Item item, ref Client client, TimeSpan loanPeriod)
                : base(item)
            {
                //Id++;
                InterestRate = loanPeriod.TotalHours * 0.05; //Зростаючий процент боргу залежно від строку
                ClientRef = client;
                _dateOfReturning = DateTime.Now.Add(loanPeriod);
            }
        }

        //Method returns true, if adding was successful
        public bool AddItem(Item item, ref Client client, TimeSpan loanPeriod)
        {
            //Checking input data
            if (item.Equals(null) || client.Name.Length < 1 || _budget > item.Value)
            {
                return false; //TODO Exceptions
            }

            _itemsList.Add(new PawnItem(item, ref client, loanPeriod));

            //TODO Exception for existing keys

            client.Budget += item.Value;

            return true;
        }

        public bool BuyItem(int index, ref Client client)
        {
            var item = _itemsList[index];
            if (_queues.ContainsKey(item.Category)) //Якщо існує черга на категорію
            {
                if (_queues[item.Category].Peek() == client) //Якщо клієнт перший в черзі
                {
                    if (client.Equals(item.ClientRef)) // Якщо це той самий клієнт
                    {
                        if (item.IsAvailableForSell) //Пізніше сроку
                        {
                            var increasedValue = item.Value + item.Value * (decimal) item.InterestRate;
                            client.Budget -= increasedValue;
                            _budget += increasedValue;
                            _itemsList.RemoveAt(index);
                            return true;
                        }

                        //До кінця сроку
                        client.Budget -= item.Value;
                        _budget += item.Value;
                        _itemsList.RemoveAt(index);
                        return true;
                    }

                    //Якщо це інший клієнт
                    if (item.IsAvailableForSell) //Якщо покупка пізніше сроку
                    {
                        client.Budget -= item.Value;
                        _budget += item.Value;
                        _itemsList.RemoveAt(index);
                        return true;
                    }
                }
            }

            EnqueueToCategory(item.Category, ref client);


            return false;
        }

        public void EnqueueToCategory(Categories category, ref Client client)
        {
            if (_queues.ContainsKey(category))
            {
                _queues[category].Enqueue(client);
            }
            else
            {
                _queues.Add(category, new Queue());
                _queues[category].Enqueue(client);
            }
        }

        public void AddClient(String name, decimal budget)
        {
            if (name.Length == 0 || budget < 0)
                throw new ArgumentException();
            _clients[name] = new Client(name, budget);
        }

        public override string ToString()
        {
            string info = $"{"Pawnshop",10} Budget: {_budget,1:C} Date: {DateTime.Now}\n";
            info += $"{"№",-3}{"ItemName",-15} {"Value",-10} {"ClientName",-15} {"Category",-15} {"IsAvailable",-15}\n";
            foreach (var item in _itemsList)
            {
                info +=
                    $"{_itemsList.IndexOf(item),-3}{item.Name,-15} {item.Value,-10} {item.ClientRef.Name,-15} {item.Category,-15} {item.IsAvailableForSell,-15}\n";
            }

            return info;
        }

        //Basic constructor
        public Pawnshop(decimal budget)
        {
            _budget = budget;
            _itemsList = new List<PawnItem>();
            _queues = new Dictionary<Categories, Queue>();
            _clients = new Dictionary<string, Client>();
        }
    }
}