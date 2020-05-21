using System;
using System.Collections;
using System.Collections.Generic;
using PawnshopNamespace;
using ItemLibrary;

namespace UserInterface
{
    static class Program
    {
        static void Main(string[] args)
        {
            Pawnshop pawnshop = new Pawnshop(10000M);
            Client bob = new Client("Bob", 1000M);
            Client bill = new Client("Bill", 5000M);
            
            Item ring = new Item("ring", 500M, Categories.Jewelry);
            Item necklace = new Item("necklace", 100M, Categories.Jewelry);
            Item lamborghini = new Item("lamborghini", 1000M, Categories.Vehicles);

            pawnshop.AddItem(ring, ref bob, new TimeSpan(0, 0, 10));
            pawnshop.AddItem(necklace, ref bill, new TimeSpan(0, 35, 0));
            pawnshop.AddItem(lamborghini, ref bob, new TimeSpan(1, 0, 0));
            
            
            Console.WriteLine(pawnshop.ToString());

            pawnshop.BuyItem(ring.Name, ref bob);
            Console.WriteLine(pawnshop.ToString());

        }
    }
}