using System;
using System.Collections.Generic;
using System.Threading;

namespace NamedPipes
{
    class StringMessage
        : IFromBytes
    {
        public string str;

        public void readFrom(ref Bytes bytes)
        {
            str = bytes.ReadString();
        }
    }

    class Program
    {
        private static string pipeName { get; } = "my_pipe";

        static void PrintMessage(StringMessage msg)
        {
            Console.WriteLine("msg: {0}", msg.str);
        }


        static void Main(string[] args)
        {
            var dispatcher = new Dispatcher();
            dispatcher.Add<StringMessage>(0, PrintMessage);

            using (var server = new Server("my_pipe", dispatcher))
            {
                server.Start();

                while(server.isWorking())
                {
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
