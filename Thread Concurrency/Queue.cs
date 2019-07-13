using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Thread_Concurrency
{
    public static class Queue
    {
        private static object[] data = new object[100];
        static int firstIndex = 0;
        static int rearIndex=0;
        static bool dequeued = false;
        static object _lock = new object();
        static object _lock2 = new object();
       
        
        static bool isEmpty()
        {
            if (firstIndex == 0 && rearIndex==0)
                return true;
            return false;
        }
       
        public static object[] getVault()
        {
            return data;
        }
        /// <summary>
        /// Counts the items present in the queue
        /// </summary>
        /// <returns></returns>
        public static int Count()
        {

            var result = dequeued ? rearIndex - 1 : rearIndex;
            return result;
        }

        public static void enQueue(object numbertoAdd)
        {
            // Could use Monitor , but thread time consumption is almost the same.
            lock (_lock)
            {
                Console.WriteLine(string.Format("{0}  enqueue enters", Thread.CurrentThread.Name));

                if (isEmpty())
                {
                    
                    data[firstIndex] = numbertoAdd;
                    rearIndex = firstIndex + 1;
                }
                else
                {

                    data[rearIndex] = numbertoAdd;
                    rearIndex = rearIndex + 1;

                }
                PrintData();
                Console.WriteLine(string.Format("{0} enqueue exits", Thread.CurrentThread.Name));

            }

        }

        public static void enQueue(object[] numbersToAdd)
        {
            lock (_lock)
            {
                Thread.Sleep(2000);
                Console.WriteLine(string.Format("{0} enqueue enters", Thread.CurrentThread.Name));
                if (isEmpty())
                {
                    foreach (var number in numbersToAdd)
                    {
                        data[rearIndex] = number;
                        rearIndex++;
                    }
                }
                else
                {
                    foreach (var number in numbersToAdd)
                    {
                        data[rearIndex++] = number;

                    }

                }
                PrintData();
                
                Console.WriteLine(string.Format("{0} enqueue exits", Thread.CurrentThread.Name));

            }
        }
        // pop the first element from the queue
        public static void deQueue() {

            lock (_lock2)
            {
                //Console.WriteLine(string.Format("{0} for dequeue enters\r\n", Thread.CurrentThread.Name));
                // POP UP THE FIRST ELEMENT
                int index = 0;
                
                while (index < rearIndex)
                {
                    data[index] = data[index + 1];
                    index++;
                }
                // set dequeued boolean variable to true to represent first item has been dequeued
                dequeued = true;
                PrintData();
                //Console.WriteLine(string.Format("{0} dequeue exits\r\n", Thread.CurrentThread.Name));

            }

        }
        /// <summary>
        /// blocks the request until timeout
        /// </summary>
        /// <param name="timeout"> in milliseconds</param>
        public static void deQueueUntilTimeOut(int timeout)
        {

            Console.WriteLine("waiting until timeout expires...");
            Thread.Sleep(timeout);
                deQueue();
            
           
        }

        /// <summary>
        /// blocks until items are available in data storage and return it
        /// </summary>
        public static object deQueueUntilReturn()
        {

            Monitor.Enter(_lock2);
            try
            {
                Thread.Sleep(1000);
                Console.WriteLine(string.Format("{0} dequeue enters", Thread.CurrentThread.Name));
                if (isEmpty())
                {
                    Console.WriteLine("the vault is empty. Waiting till the objects are available");

                    Console.WriteLine("blocking the current thread " + Thread.CurrentThread.Name);

                    //release the lock and block the current thread until the lock is acquire again
                    Monitor.Wait(_lock2);
                }
                else
                {
                    deQueue();
                    // Notifies the waiting thread about the change in the state of lock?
                    Monitor.PulseAll(_lock2);
                    

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception is:", ex);
            }
            finally
            {
                if (Thread.CurrentThread.Name == "Vault Manager")
                {
                    Console.WriteLine("===Vault Manager : You have new items in your vault===\r\n");
                }
                Console.WriteLine(string.Format("{0} dequeue exits",Thread.CurrentThread.Name));
                Monitor.Exit(_lock2);
                
            }
            var vault = getVault();
            return vault;

           
        }
        /// <summary>
        /// dequeues all the items in the queue
        /// </summary>
        public static void deQueueAll()
        {
            lock (_lock2)
            {
                if (isEmpty())
                {
                    Console.WriteLine("No items to dequeue");
                }
                else
                {
                    int index = 0;
                    // NEED TO CHECK IF ANY ITEM HAS BEEN PREVIOUSLY DEQUEUED
                    if (dequeued)
                    {
                        // deduced the already dequeued item Note: we can only dequeue the one item FIFO
                        while (index < rearIndex - 1)
                        {
                            Console.WriteLine(string.Format("item dequeued {0} at index {1}", data[index], index));
                            data[index] = null;
                            index++;
                        }

                    }
                    else
                    {
                        while (index < rearIndex)
                        {
                            Console.WriteLine(string.Format("item dequeued {0} at index {1}", data[index], index));
                            data[index] = null;
                            index++;
                        }
                    }

                    // Notify users that all items have been dequeued;
                    Console.WriteLine("all items dequeued");

                    firstIndex = 0;
                    rearIndex = 0;
                    PrintData();
                }
            }
        }
        
       
        /// <summary>
        /// Printing helper function
        /// </summary>
        static void PrintData()
        {
            if (!isEmpty())
            {
                Console.Write("items left in the vault : ");
                int index = 0;
                if (dequeued)
                {
                    // remove one item from the data to represent the item removed
                    while (index <= rearIndex - 1)
                    {
                        Console.Write(data[index] + " ");
                        index++;
                    }
                    Console.Write(" end\r\n");
                }
                else
                {
                    while (index <= rearIndex)
                    {
                        Console.Write(data[index] + " ");
                        index++;
                    }
                    Console.Write(" <<end \r\n");
                  
                }
            }
            else
            {
                Console.Write("no items to print \r\n");
            }
            
           
        }
      
       
    }

    
    


   

    
}
