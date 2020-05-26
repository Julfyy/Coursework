using System;
using System.Globalization;
using PawnshopNamespace;
using ItemLibrary;

namespace UserInterface
{
    static class Program
    {
        private static string History = "";
        static void Main()
        {
            String CommandList = "\tCOMMANDS LIST:\n" +
                                 "\t\"commands\" to show commands list\n" +
                                 "\t\"newclient\" to register a new client\n" +
                                 "\t\"additem\" to add your item to the pawnshop\n" +
                                 "\t\"enqueue\" to take the last place in a queue of come category\n" +
                                 "\t\"buyitem\" to buy an item (if you are the first in the queue)\n" +
                                 "\t\"history\" to show history of  transactions\n" +
                                 "\t\"info\" to see actual information\n" +
                                 "\t\"exit\" to end up a session";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hello! Welcome to the PawnShop!");
            Console.WriteLine(CommandList);
            Console.ResetColor();
            Pawnshop pawnshop = new Pawnshop(100000M);
            pawnshop.Notify += DisplayMessage;
            pawnshop.Notify += WriteToHistory;
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
                            }
                            break;

                        case "additem":
                            //Виведення інформації про параметри к консоль
                            Console.WriteLine("Enter: item's name, value, category, client's name and loan period (DD:HH:MM:SS)\n" +
                                              "\t(e.g. ring 100 0 bob 0:0:5:0)\n" +
                                              "Categories:");
                            //Виведення списку категорій
                            foreach (var s in Enum.GetNames(typeof(Categories)))
                            {
                                Console.WriteLine("{0,15} = {1}", s, Enum.Format(typeof(Categories),
                                                    Enum.Parse(typeof(Categories), s), "d"));
                            }

                            try
                            {
                                var inputSplit = Console.ReadLine().Split(' ');
                                
                                //Перевірка на правильність вводу категорії
                                if (Enum.TryParse( inputSplit[2], true, out Categories category)) 
                                {
                                    if (!Enum.IsDefined(typeof(Categories), category))
                                    {
                                        Console.WriteLine("Wrong category number!");
                                        break;
                                    }
                                }

                                pawnshop.AddItem(inputSplit[0], decimal.Parse(inputSplit[1]), category,
                                                                    pawnshop.GetClientRef(inputSplit[3]),
                                                                    TimeSpan.Parse(inputSplit[4]));
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
                                pawnshop.BuyItem(int.Parse(inputSplit[0]), pawnshop.GetClientRef(inputSplit[1]));
                            }
                            catch (Pawnshop.QueueException e)
                            {
                                Console.WriteLine($"{e.Message}");
                            }
                            catch (InvalidTimeZoneException e)
                            {
                                Console.WriteLine($"{e.Message}");
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
                                Console.WriteLine($"{e.Message}");
                            }
                            break;
                        case "dequeue":
                            Console.WriteLine("Enter: category and client's name\n" +
                                              "(You can dequeue only when you are the first");
                            try
                            {
                                var inputSplit = Console.ReadLine().Split(' ');
                                Enum.TryParse(inputSplit[0], out Categories category);
                                pawnshop.DequeueFromCategory(category, pawnshop.GetClientRef(inputSplit[1]));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"{e.Message}");
                            }
                            break;

                        case "info":
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(pawnshop.ToString());
                            Console.ResetColor();
                            break;
                        case "history":
                            Console.WriteLine(History);
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

        private static void DisplayMessage(object sender, Pawnshop.PawnshopEventArgs e)
        {
            Console.WriteLine($"{e.Message}");
        }

        private static void WriteToHistory(object sender, Pawnshop.PawnshopEventArgs e)
        {
            History += $"{e.Message} at {DateTime.Now}\n";
        }
        
    }
}