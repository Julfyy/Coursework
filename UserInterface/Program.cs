using System;
using PawnshopNamespace;
using ItemLibrary;

namespace UserInterface
{
    static class Program
    {
        static void Main(string[] args)
        {
            Pawnshop pawnshop = new Pawnshop(1000M);

            Client bob = new Client("Bob", 100M);
            Item ring = new Item("ring", 50M, Categories.Jewelry);

            pawnshop.AddItem(ring, ref bob, 0.01, new TimeSpan(0, 5, 0));

            Client bill = new Client("Bill", 5000M);
            pawnshop.BuyItem(ring, ref bill);
            Console.WriteLine();
        }
    }
}