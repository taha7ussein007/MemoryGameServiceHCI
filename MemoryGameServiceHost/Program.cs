using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace MemoryGameServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;

            if (args.Length < 1) 
            {
                //cannot start
                Console.WriteLine("Host cannot Started!!!");
                Console.ReadLine();
            }
            else
            {
                try
                {
                    //args[0] = IP
                    Uri baseAddress = new Uri("http://" + args[0] + ":8888/MemoryGameService/MemoryGameService/");
                    Console.WriteLine(Convert.ToString(baseAddress));
                    host = new ServiceHost(typeof(MemoryGameService.MemoryGameService), baseAddress);
                    host.Open();
                    
                    Console.WriteLine("Host Started @ " + args[0] + ":8888" + " ...\n\n > Enter <end> to terminate the server.");

                    string str = "";

                    while (str != "end")
                    {
                        str = Console.ReadLine();
                        if (str == "end")
                            host.Close();
                    }

                }
                catch (Exception)
                {
                    //cannot start
                    Console.WriteLine("Host cannot Started @ " + args[0]);
                    Console.WriteLine("\n >> Maybe the host has already started \n    or you don't have administrator privileges.");
                    Console.ReadLine();
                }
            }
            try
            {
                host.Close();
                return;
            }
            catch (Exception) { return; }
        }
    }
}
