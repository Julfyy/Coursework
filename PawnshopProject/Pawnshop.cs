using System;
using System.Collections.Generic;
using ItemLibrary;

namespace PawnshopNamespace
{
    public class Pawnshop
    {
        private decimal _budget;
        private readonly List<PawnItem> _itemsList;
        private readonly Dictionary<Categories, Queue<Client>> _queues;
        private readonly Dictionary<String, Client> _clients;

        private class PawnItem : Item
        {
            public bool IsAvailableForSell
            {
                get
                {
                    if (DateTime.Now.CompareTo(_dateOfReturning) > 0)
                        return true;
                    return false;
                }
            }
            private readonly DateTime _dateOfReturning;
            internal readonly Client ClientRef;
            internal readonly double InterestRate;

            internal PawnItem(String name, decimal value, Categories category, ref Client client, TimeSpan loanPeriod)
                : base(name, value, category)
            {
                InterestRate = loanPeriod.TotalHours * 0.05; //Зростаючий процент боргу залежно від строку
                ClientRef = client;
                _dateOfReturning = DateTime.Now.Add(loanPeriod);
            }
        }

        //Method returns true, if adding was successful
        public bool AddItem(String name, decimal value, Categories category, Client client, TimeSpan loanPeriod)
        {
           _itemsList.Add(new PawnItem(name, value, category, ref client, loanPeriod));
           _budget -= value;
           client.Budget += value;
           return true;
        }

        public bool BuyItem(int index, Client client)
        {
            var item = _itemsList[index];
            if (_queues.ContainsKey(item.Category)) //Якщо існує черга на категорію
            {
                if (_queues[item.Category].Peek().Equals(client)) //Якщо клієнт перший в черзі
                {
                    if (client.Equals(item.ClientRef)) // Якщо це той самий клієнт
                    {
                        if (item.IsAvailableForSell) //Пізніше сроку
                        {
                            var increasedValue = item.Value + item.Value * (decimal) item.InterestRate;
                            client.Budget -= increasedValue;
                            _budget += increasedValue;
                            _itemsList.RemoveAt(index);
                            _queues[item.Category].Dequeue();
                            return true;
                        }

                        //До кінця сроку
                        client.Budget -= item.Value;
                        _budget += item.Value;
                        _itemsList.RemoveAt(index);
                        _queues[item.Category].Dequeue();
                        return true;
                    }

                    //Якщо це інший клієнт
                    if (item.IsAvailableForSell) //Якщо покупка пізніше сроку
                    {
                        client.Budget -= item.Value;
                        _budget += item.Value;
                        _itemsList.RemoveAt(index);
                        _queues[item.Category].Dequeue();
                        return true;
                    }
                }
                return false;
            }

            EnqueueToCategory(item.Category, client); //Якщо черга пуста, створити її

            if (BuyItem(index, client)) //Тоді поточний клієнт буде першим, тобто може купити річ
                return true;
        
            return false;
        }

        public void EnqueueToCategory(Categories category, Client client)
        {
            if (_queues.ContainsKey(category)) //Якщо черга на задану категорію існує
                _queues[category].Enqueue(client);
            
            else
            {
                _queues.Add(category, new Queue<Client>());
                _queues[category].Enqueue(client);
            }
        }

        public void AddClient(String name, decimal budget)
        {
            if (name.Length == 0 || budget < 0)
                throw new ArgumentException();
            _clients.Add(name, new Client(name, budget));
        }

        public Client GetClientRef(String name)
        {
            return _clients[name];
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
            
            info += "\nClients list:\n";
            foreach (var client in _clients.Keys)
            {
                info += $"{client}\n";
            }

            info += "\nCurrent queues:\n";
            foreach (var category in _queues.Keys)
            {
                info += $"{category}:\n";
                foreach (var client in _queues[category].ToArray())
                {
                    info += $"\t{client.Name}\n";
                }
            }
            
            return info;
        }

        //Basic constructor
        public Pawnshop(decimal budget)
        {
            _budget = budget;
            _itemsList = new List<PawnItem>();
            _queues = new Dictionary<Categories, Queue<Client>>();
            _clients = new Dictionary<string, Client>();
        }
    }
}