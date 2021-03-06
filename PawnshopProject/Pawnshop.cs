using System;
using System.Collections.Generic;
using ItemLibrary;

namespace PawnshopNamespace
{
    public class Pawnshop
    {
        private decimal _budget;                        //Бюджет ломбарду
        private readonly List<PawnItem> _itemsList;     //Ліст колекція об'єктів PawnItem, що зберігає їх по індексу
        private readonly Dictionary<Categories, Queue<Client>> _queues; //Колекція - словник черг клієнтів до кожної категорії
        private readonly Dictionary<String, Client> _clients; //Колекція - словник клієнтів, за парами: ім'я - посилання на об'єкт 

        public delegate void EventHandler(object sender, PawnshopEventArgs e);
        public event EventHandler Notify;

        /*
         * Внутрішній клас, що наслідує Item для збереження інформації про товар
         * Має поля: доступний для продажу, дата повернення, посилання на клієнта
         * Поля наслідувані з Item: назва товару, ціна, категорія
         */
        private class PawnItem : Item
        {
            public bool IsAvailableForSell              //Чи товар доступний для продажу
            {
                get
                {   //Доступний якщо термін закладення збіг
                    if (DateTime.Now.CompareTo(DateOfReturning) >= 0) 
                        return true;
                    return false;
                }
            }
            internal readonly DateTime DateOfReturning; //Збереження дати повернення
            internal readonly Client ClientRef;         //Посилання на об'єкт клієнта - власника товару
            internal readonly double InterestRate;      //ВІдсоткова ставка
            
            //Конструктор класу PawnItem
            internal PawnItem(String name, decimal value, Categories category, ref Client client, TimeSpan loanPeriod)
                : base(name, value, category)
            {
                InterestRate = loanPeriod.TotalHours * 0.05; //Зростаючий процент боргу залежно від строку
                ClientRef = client;
                DateOfReturning = DateTime.Now.Add(loanPeriod); //Дата повернення = поточна дата + період закладу
            }
        }

        /*
        Метод продажу товарів до ломбарду за певну суму грошей
        Параметри: назва товару, ціна, категорія, посилання на об'єкт клієнта, період закладення в ломбард
        */
        public void AddItem(String name, decimal value, Categories category, Client client, TimeSpan loanPeriod)
        
        {
            //Якщо ціна товару від'ємна - виключення
            if (value < 0)     
                throw new ArgumentException("Found negative value for item");
            
            //Додаю об'єкт PawnItem до списку товарів
            _itemsList.Add(new PawnItem(name, value, category, ref client, loanPeriod)); 
            
            _budget -= value; //З рахунку ломбарду знімається сума
            client.Budget += value; //Сума додається на рахунок клієнта
            
            //Івент, що сповіщає про успішне додавання товару
            Notify?.Invoke(this, new PawnshopEventArgs($"{client.Name} added {name} to pawnshop for {value, 1:C}"));
        }

        /*
         * Основний метод покупки товару з ломбарду
         * Параметрами передаються індекс товару у списку та лінк на об'єкт клієнта
         */
        public void BuyItem(int index, Client client)
        {
            var item = _itemsList[index]; //ВНутрішня змінна для збереження об'єкту товару
            if (_queues.ContainsKey(item.Category) && _queues[item.Category]?.Count != 0) //Якщо існує не пуста черга на категорію
            {
                if (client.Equals(item.ClientRef)) // Якщо це той самий клієнт
                {
                    if (item.IsAvailableForSell) //Пізніше сроку
                    {
                        //В цьому випадку товар продається за збільшеною на відсоток ціною
                        var increasedValue = item.Value + item.Value * (decimal) item.InterestRate;
                        client.Budget -= increasedValue;
                        _budget += increasedValue;
                        _itemsList.RemoveAt(index);
                        Notify?.Invoke(this, new PawnshopEventArgs($"{client.Name} bought {item.Name} for {increasedValue, 1:C}" ));
                        return;
                    }

                    //Раніше сроку
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
                    //Якщо покупка раніше сроку
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

        //Метод для того щоб зайняти чергу на певну категорію товару
        public void EnqueueToCategory(Categories category, Client client)
        {
            
            if (_queues.ContainsKey(category))  //Якщо черга на задану категорію існує
            {
                if (_queues[category].Contains(client)) //Якщо клієнт вже в черзі
                    throw new QueueException("You are already in the queue!");
                _queues[category].Enqueue(client);
                Notify?.Invoke(this, new PawnshopEventArgs($"Added {client.Name} to queue for category {category}"));
            }
            else //Якщо черги на категорію не існує
            {
                _queues.Add(category, new Queue<Client>());
                _queues[category].Enqueue(client);
                Notify?.Invoke(this, new PawnshopEventArgs($"Created a new queue for {category} and added {client.Name} first"));
            }
        }
        
        //Метод для того, щоб вийти з черги, не купуючи товар
        public void DequeueFromCategory(Categories category, Client client)
        {
            if (_queues[category].Contains(client) && _queues[category].Peek().Equals(client)) //Якщо в черзі є клієнт і він перший
            {
                _queues[category].Dequeue();
                Notify?.Invoke(this, new PawnshopEventArgs($"Client {client.Name} left the queue to category {category}"));
            }
            else //Якщо клієнта немає в черзі, або він не перший
            {
                throw new QueueException("You are not in the queue or not in the first place!");
            }
            
        }

        //Допоміжний метод для додавання нового клієнта
        public void AddClient(String name, decimal budget)
        {
            if (name.Length == 0 || budget < 0) //Перевірка введених даних
                throw new ArgumentException();
            _clients.Add(name, new Client(name, budget));
            Notify?.Invoke(this, new PawnshopEventArgs($"Added new client {name}"));
        }

        //Допоміжний метод, щоб отримати посилання на об'єкт клієнта з його іменем.
        public Client GetClientRef(String name)
        {
            return _clients[name];
        }
        

        //Метод для вивдення поточної інформації про стан ломбарду
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

        //Базовий конструктор класу Pawnshop
        public Pawnshop(decimal budget)
        {
            _budget = budget;
            _itemsList = new List<PawnItem>();
            _queues = new Dictionary<Categories, Queue<Client>>();
            _clients = new Dictionary<string, Client>();
        }

        //Виключення для черги
        public class QueueException : Exception
        {
            public QueueException(String mes) : base(mes)
            {
                
            }
        }
        
        //Клас для передання інформації про івент
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