using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_Concurrency
{
    public class Program
    {
        public static void Main(string[] args)
        {

            SimpleConsole();
            //DequeueUntilReturn();

        }

        /// <summary>
        ///  A simple Application Programming interface for users to queue and dequeue their items.
        /// </summary>
        public static void SimpleConsole()
        {
            do
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("====Welcome to the Vault...");
                Console.WriteLine();
                Console.WriteLine("Press 1 to add items to your vault.");
                Console.WriteLine();
                Console.WriteLine("Press 2 to dequeue an item from your vault");
                Console.WriteLine("Press 3 to dequeue all items from your vault");
                Console.WriteLine("Press 4 to view all items in your vault");

                Console.WriteLine();
                Console.WriteLine("Press e to exit");
                
                switch (Console.ReadLine())
                {
                    case "1":
                        {
                            Console.WriteLine("Welcome, add your items here followed by comma");
                            string input = Console.ReadLine();
                            input = input.Contains(",") ? Regex.Replace(input, @"\s+", "") : input;

                            Thread t = new Thread(() => {
                                Console.WriteLine("Adding to the vault !!\r\n");
                                Queue.enQueue(input.Split(','));
                            });
                            t.Start(); t.Join();

                            break;
                        }
                    case "2":
                        {
                            
                            if (Queue.Count() == 0)
                            {
                                Console.WriteLine("No items in the vault, Do you want Vault Manager to notify when the vault fills next time when you dequeue ?\r\n");
                                Console.WriteLine("Press y or n");
                                var input=Console.ReadLine();
                                switch (input)
                                {
                                    case "y":
                                        {
                                            Console.WriteLine("Manager: I will let you know sir");
                                            // run a task in asynchronous manner
                                            Thread t1 = new Thread(() => { Queue.deQueueUntilReturn(); });
                                            t1.Name = "Vault Manager";
                                            t1.Start();
                                            Thread.Sleep(2000);

                                            
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                var count = Queue.Count();
                                Queue.deQueueUntilReturn();
                            }
                            break;


                        }
                    case "e":
                        {
                            Console.WriteLine("Good Bye !!!! ('_')");
                            return;
                        }
                    case "3":
                        {
                            Queue.deQueueAll();

                            break;
                        }
                    case "4":
                        {
                            DisplayAllItems();
                            break;
                        }

                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Press q to go back to main menu or press e to exit");
            }
            while (Console.ReadKey() != new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false));



        }

        private static void DisplayAllItems()
        {
            var data = Queue.getVault();
            var count = Queue.Count();
            if (count == 0)
            {
                Console.WriteLine("No items to display \r\n");
                return;
            }
            int index = 0;
            Console.Write("Items in the vault are: ");
            while (index < count)
            {
                Console.Write(data[index] + " ");
                index++;
                
            }
            Console.Write("<<end\r\n");
        }

        private static object[] GetItemsFromInput(string input)
        {
            var nowhitespaces = Regex.Replace(input, @"\s+", "");
            var data = nowhitespaces.Split(',');

            return data;
        }


    }


}
