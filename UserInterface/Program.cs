using System;
using PawnshopNamespace;
using ItemLibrary;

namespace UserInterface
{
    static class Program
    {
        static void Main()
        {
            String CommandList = "\tCOMMANDS LIST:\n" +
                                 "\t\"commands\" to show commands list\n" +
                                 "\t\"newclient\" to register a new client\n" +
                                 "\t\"additem\" to add your item to the pawnshop\n" +
                                 "\t\"enqueue\" to take the last place in a queue of come category\n" +
                                 "\t\"buyitem\" to buy an item (if you are the first in the queue)\n" +
                                 "\t\"info\" to see actual information\n" +
                                 "\t\"exit\" to end up a session";
            
            Console.WriteLine("To start type \"start\"");
            String command = Console.ReadLine();
            if (command == "start")
            {
                Console.WriteLine("Hello! Welcome to the PawnShop!");
                Pawnshop pawnshop = new Pawnshop(100000M);
                Console.WriteLine(CommandList);
                while (true)
                {
                    {
                        command = Console.ReadLine();
                        switch (command)
                        {
                            case "newclient":
                                Console.WriteLine("Enter client's name and budget (e.g. bob 1000):");
                                try
                                {
                                    var inputSplit = Console.ReadLine().Split(' ');
                                    pawnshop.AddClient(inputSplit[0], decimal.Parse(inputSplit[1]));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Incorrect input! {e.Message}");
                                    break;
                                }
                                Console.WriteLine($"Successfully added new client");
                                break;
                            
                            case "additem":
                                Console.WriteLine("Enter: item's name, value, category, client's name and loan period\n" +
                                                  "e.g. ring 50 jewelry bob 0:0:5");
                                try
                                {
                                    var inputSplit = Console.ReadLine().Split(' ');
                                    Enum.TryParse(inputSplit[2], out Categories category);
                                    pawnshop.AddItem(inputSplit[0],
                                        decimal.Parse(inputSplit[1]),
                                        category,
                                        pawnshop.GetClientRef(inputSplit[3]),
                                        TimeSpan.Parse(inputSplit[4]));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Incorrect input! {e.Message}");
                                    break;
                                }
                                Console.WriteLine("Successfully added new item to pawnshop");
                                break;
                            
                            case "info":
                                Console.WriteLine(pawnshop.ToString());
                                break;
                            case "commands":
                                Console.WriteLine(CommandList);
                                break;
                            case "exit":
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("Unrecognized command");
                                break;
                        }
                    }
                }
            }


            /*
            
            Client bob = new Client("Bob", 1000M);
            Client bill = new Client("Bill", 5000M);
            
            
            Item ring = new Item("ring", 500M, Categories.Jewelry);
            Item necklace = new Item("necklace", 100M, Categories.Jewelry);
            Item lamborghini = new Item("lamborghini", 1000M, Categories.Vehicles);

            pawnshop.AddItem(ring, ref bob, new TimeSpan(0, 0, 10));
            pawnshop.AddItem(necklace, ref bill, new TimeSpan(0, 35, 0));
            pawnshop.AddItem(lamborghini, ref bob, new TimeSpan(1, 0, 0));
            
            Console.WriteLine(pawnshop.ToString());

            //pawnshop.EnqueueToCategory(Categories.Jewelry, ref bob);
            pawnshop.BuyItem(0, ref bob);
            pawnshop.BuyItem(0, ref bob);
            
            
            Console.WriteLine(pawnshop.ToString());
*/
        }
    }
}