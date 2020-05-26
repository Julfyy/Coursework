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

        public delegate void EventHandler(object sender, PawnshopEventArgs e);
        public event EventHandler Notify;

        private class PawnItem : Item
        {
            public bool IsAvailableForSell
            {
                get
                {
                    if (DateTime.Now.CompareTo(DateOfReturning) >= 0)
                        return true;
                    return false;
                }
            }
            internal readonly DateTime DateOfReturning;
            internal readonly Client ClientRef;
            internal readonly double InterestRate;

            internal PawnItem(String name, decimal value, Categories category, ref Client client, TimeSpan loanPeriod)
                : base(name, value, category)
            {
                InterestRate = loanPeriod.TotalHours * 0.05; //Зростаючий процент боргу залежно від строку
                ClientRef = client;
                DateOfReturning = DateTime.Now.Add(loanPeriod);
            }
        }

        //Method returns true, if adding was successful
        public void AddItem(String name, decimal value, Categories category, Client client, TimeSpan loanPeriod)
        {
            if (value < 0)
                throw new ArgumentException("Found negative value for item");
            _itemsList.Add(new PawnItem(name, value, category, ref client, loanPeriod));
            _budget -= value;
            client.Budget += value;
            Notify?.Invoke(this, new PawnshopEventArgs($"{client.Name} added {name} to pawnshop for {value, 1:C}"));
        }

        public void BuyItem(int index, Client client)
        {
            var item = _itemsList[index];
            if (_queues.ContainsKey(item.Category) && _queues[item.Category]?.Count != 0) //Якщо існує не пуста черга на категорію
            {
                if (client.Equals(item.ClientRef)) // Якщо це той самий клієнт
                {
                    if (item.IsAvailableForSell) //Пізніше сроку
                    {
                        var increasedValue = item.Value + item.Value * (decimal) item.InterestRate;
                        client.Budget -= increasedValue;
                        _budget += increasedValue;
                        _itemsList.RemoveAt(index);
                        Notify?.Invoke(this, new PawnshopEventArgs($"{client.Name} bought {item.Name} for {increasedValue, 1:C}" ));
                        return;
                    }

                    //До кінця сроку
                    client.Budget -= item.Value;
                    _budget += item.Value;
                    _itemsList.RemoveAt(index);
                    Notify?.Invoke(this, new PawnshopEventArgs($"{client.Name} bought {item.Name} for {item.Value, 1:C}"));
                    return;
                }
                //Якщо це інший клієнт (Тоді перевірка на чергу)
                if (_queues[item.Category].Peek().Equals(client)) //Якщо клієнт перший в черзі
                {
                    if (item.IsAvailableForSell) //Якщо покупка пізніше сроку
                    {
                        client.Budget -= item.Value;
                        _budget += item.Value;
                        _itemsList.RemoveAt(index);
                        _queues[item.Category].Dequeue();
                        Notify?.Invoke(this, new PawnshopEventArgs($"{client.Name} bought {item.Name} for {item.Value, 1:C}"));
                        return;
                    }
                    throw new InvalidTimeZoneException("You are not able to buy this item yet! Wait till it's available!");
                }
                //Якщо клієнта взагалі немає в черзі
                if (!_queues[item.Category].Contains(client))
                    throw new QueueException("Enqueue to the category first!");
                //Якщо клієнт не перший в черзі
                throw new QueueException("You are not the first is the queue!");
            }
            //Якщо черги не існує або існує пуста
            Notify?.Invoke(this, new PawnshopEventArgs("Queue for this category has not found. Creating a queue automatically"));
            EnqueueToCategory(item.Category, client); //Якщо черги немає, створити її
            BuyItem(index, client); //Тоді поточний клієнт буде першим, тобто може купити річ
        }

        public void EnqueueToCategory(Categories category, Client client)
        {
            if (_queues.ContainsKey(category))  //Якщо черга на задану категорію існує
            {
                _queues[category].Enqueue(client);
                Notify?.Invoke(this, new PawnshopEventArgs($"Added {client.Name} to queue for category {category}"));
            }
            else
            {
                _queues.Add(category, new Queue<Client>());
                _queues[category].Enqueue(client);
                Notify?.Invoke(this, new PawnshopEventArgs($"Created a new queue for {category} and added {client.Name} first"));
            }
        }

        public void AddClient(String name, decimal budget)
        {
            if (name.Length == 0 || budget < 0)
                throw new ArgumentException();
            _clients.Add(name, new Client(name, budget));
            Notify?.Invoke(this, new PawnshopEventArgs($"Added new client {name}"));
        }

        public Client GetClientRef(String name)
        {
            return _clients[name];
        }
        

        public override string ToString()
        {
            string info = $"{"Pawnshop",10} Budget: {_budget,1:C} Date: {DateTime.Now}\n";
            info += $"{"№",-3}{"ItemName",-15} {"Value",-10} {"ClientName",-15}" +
                    $" {"Category",-15} {"IsAvailable",-15} {"Loan Date", -15}\n";
            foreach (var item in _itemsList)
            {
                info += $"{_itemsList.IndexOf(item),-3}{item.Name,-15} {item.Value,-10:C} " +
                        $"{item.ClientRef.Name,-15} {item.Category,-15}" +
                        $" {item.IsAvailableForSell,-15} {item.DateOfReturning}\n";
            }
            
            info += "\nClients:\n";
            foreach (var client in _clients.Values)
            {
                info += $"{client.Name, 15} {client.Budget, 1:C}\n";
            }

            info += "\nCurrent queues:\n";
            foreach (var category in _queues.Keys)
            {
                info += $"{category} :\n";
                foreach (var client in _queues[category].ToArray())
                {
                    info += $"{client.Name}\n";
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

        public class QueueException : Exception
        {
            public QueueException(String mes) : base(mes)
            {
                
            }
        }

        public class PawnshopEventArgs
        {
            public string Message {get;}
           

            public PawnshopEventArgs(string mes)
            {
                Message = mes;
                
            }
        }
    }
}