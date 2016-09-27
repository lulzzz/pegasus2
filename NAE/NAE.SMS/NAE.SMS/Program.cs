using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NAE.SMS
{
    class Program
    {
        private static string userMessageTopicUriString = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/usermessage";
        private static string issuer = "urn:pegasusmission.io";
        private static string audience = "http://broker.pegasusmission.io/api/connect";
        private static string signingKey = "cW0iA3P/mhFi0/O4EAja7UuJ16q6Aeg4cOzL7SIvLL8=";

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Activate/Deactivate/Skip SMS (A/D/[Enter]) ? ");
            string result = Console.ReadLine().ToLower();
            Console.ResetColor();


            if (result == "d")
            {
                DeactivateSms();

            }


            if (result == "a")
            {
                ActivateSms();
            
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Do you want to send a message (Y/N) ? ");
            result = Console.ReadLine().ToLower();

            if (result == "y")
            {
                Console.Write("Do you want to Type it or read from File (T/F) ? ");
                result = Console.ReadLine().ToLower();
                if (result == "t")
                {
                    Console.Write("Enter message <= 160 chars : ");
                    string message = Console.ReadLine();
                    Console.ResetColor();
                    SendMessage(message);
                }
                if (result == "f")
                {
                    Console.Write("Enter path to file: ");
                    string path = Console.ReadLine();
                    Console.ResetColor();
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string fileMessage = reader.ReadToEnd();
                        Console.WriteLine(fileMessage);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Send this message (Y/N) ");
                        result = Console.ReadLine().ToLower();
                        Console.ResetColor();

                        if (result == "y")
                        {
                            SendMessage(fileMessage);
                        }
                    }
                }
            }


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();           

        }
        
        private static void ActivateSms()
        {
            StorageManager sm = new StorageManager();
            List<PhoneEntity> list = sm.GetFalsePhones();

            if (list == null || list.Count == 0)
            {
                Console.WriteLine("List activated.");
                return;
            }

            int index = 1;
            List<Task> taskList = new List<Task>();
            foreach (PhoneEntity entity in list)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    entity.Activated = true;
                    sm.Update(entity);
                    Console.WriteLine("Updated {0} of {1}", index, list.Count);
                    index++;
                });

                taskList.Add(task);
                
            }

            if(taskList.Count > 0)
            {
                Task.WaitAll(taskList.ToArray());
            }

            Console.WriteLine("List activated.");
        }

        private static void DeactivateSms()
        {
            StorageManager sm = new StorageManager();
            List<PhoneEntity> list = sm.GetTruePhones();


            if(list == null || list.Count == 0)
            {
                Console.WriteLine("List deactivated.");
                return;
            }

            List<Task> taskList = new List<Task>();

            int index = 1;
            foreach (PhoneEntity entity in list)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    entity.Activated = false;
                    sm.Update(entity);
                    Console.WriteLine("Updated {0} of {1}", index, list.Count);
                    index++;
                });

                taskList.Add(task);
                
            }

            if(taskList.Count > 0)
            {
                Task.WhenAll(taskList.ToArray());
            }

            Console.WriteLine("List deactivated.");
        }

        public static void SendMessage(string message)
        {
            try
            {
                StorageManager manager = new StorageManager();
                List<Task> taskList = new List<Task>();
                List<PhoneEntity> phones = manager.GetTruePhones();
                int index = 1;
                if (phones != null && phones.Count > 0)
                {
                    foreach (PhoneEntity phone in phones)
                    {
                        if (phone.Activated)
                        {
                            TextMessage tmessage = new TextMessage();
                            //Task task = tmessage.NotifyAsync(message, phone.Phone);
                            //taskList.Add(tmessage.NotifyAsync(message, phone.Phone));
                            Task task = Task.Factory.StartNew(async () =>
                            {
                                await tmessage.NotifyAsync(message, phone.Phone);
                            });

                            Task.WhenAll(task);
                            Console.WriteLine("Sent message {0} of {1}", index, phones.Count);
                            index++;

                            //if(index%5 == 0)
                            //{
                            //    Task.WhenAll(taskList.ToArray());
                            //    taskList = new List<Task>();
                            //    Console.WriteLine("Sent messages {0} - {1} of {2}", index - 5, index, phones.Count);
                            //}
                        }
                    }

                    //if(taskList.Count > 0)
                    //{
                    //    Task.WhenAll(taskList.ToArray());                       
                    //}
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("SMS notifications faulted.");
                Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
