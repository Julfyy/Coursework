﻿using System;
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


            Console.WriteLine("Hello! Welcome to the PawnShop!");
            Pawnshop pawnshop = new Pawnshop(100000M);
            Console.WriteLine(CommandList);
            while (true)
            {
                {
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "newclient":
                            Console.WriteLine("Enter client's name and budget (e.g. bob 1000.0):");
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
                            Console.WriteLine("Enter: item's name, value, category, client's name and loan period (DD:HH:MM:SS)\n" +
                                              "\te.g. ring 100 0 bob 0:0:5:0\n" +
                                              "Categories:");
                            foreach (var s in Enum.GetNames(typeof(Categories)))
                                Console.WriteLine("{0,15} = {1}", s, Enum.Format(typeof(Categories), Enum.Parse(typeof(Categories), s), "d"));
                            try
                            {
                                var inputSplit = Console.ReadLine().Split(' ');
                                Enum.TryParse(inputSplit[2], out Categories category);
                                if(pawnshop.AddItem(inputSplit[0], decimal.Parse(inputSplit[1]), category,
                                                                            pawnshop.GetClientRef(inputSplit[3]),
                                                                            TimeSpan.Parse(inputSplit[4])))
                                    Console.WriteLine("Successfully added new item to pawnshop");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Incorrect input! {e.Message}");
                                
                            }
                            break;
                        
                        case "buyitem":
                            Console.WriteLine("Enter: index of an item in the list and client's name");
                            try
                            {
                                var inputSplit = Console.ReadLine().Split(' ');
                                if (pawnshop.BuyItem(int.Parse(inputSplit[0]), pawnshop.GetClientRef(inputSplit[1])))
                                    Console.WriteLine("Successfully sold item ");
                                else
                                    Console.WriteLine("Enqueue to category first!");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Incorrect input! {e.Message}");
                            }
                            break;
                        case "enqueue":
                            Console.WriteLine("Enter: category and client's name");
                            try
                            {
                                var inputSplit = Console.ReadLine().Split(' ');
                                Enum.TryParse(inputSplit[0], out Categories category);
                                pawnshop.EnqueueToCategory(category, pawnshop.GetClientRef(inputSplit[1]));

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Incorrect input! {e.Message}");
                                break;
                            }
                            Console.WriteLine("Successfully added client to the queue");
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
    }
}