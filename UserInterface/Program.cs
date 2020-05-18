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
            Pawnshop pawnshop = new Pawnshop(1000M);

            Client bob = new Client("Bob", 100M);
            Item ring = new Item("ring", 500M, Categories.Jewelry);
            Item necklace = new Item("necklace", 100M, Categories.Jewelry);
            Client bill = new Client("Bill", 5000M);


            pawnshop.AddItem(ring, ref bob, new TimeSpan(0, 5, 0));
            pawnshop.AddItem(necklace, ref bill, new TimeSpan(0, 35, 0));


            pawnshop.BuyItem(ring.Name, ref bill);
            Console.WriteLine(pawnshop.Budget);
        }
    }
}