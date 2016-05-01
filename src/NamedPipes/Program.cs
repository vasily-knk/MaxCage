using System;
using System.Threading;

namespace NamedPipes
{
    class Program
    {
        private static string pipeName { get; } = "my_pipe";

        static void Main(string[] args)
        {
            using (var server = new Server(pipeName))
            {
                while(server.isWorking)
                {
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
