using System;

namespace PawnshopNamespace
{
    /*
     * Клас що реалізує клієнта ломбарду
     * Має поля: ім'я та бюджет
     * Базовий конструктор
     */
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