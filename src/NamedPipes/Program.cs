using System;
using System.Threading;
using System.IO.Pipes;

namespace NamedPipes
{
    class Program
    {
        private static string pipeName { get; } = "my_pipe";

        private static void Server()
        {
            Console.WriteLine("Server started");

            using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.In))
            {
                Console.WriteLine("Waiting for connection");
                pipe.WaitForConnection();

                Console.WriteLine("Reading byte");
                int result = pipe.ReadByte();

                Console.WriteLine("Byte read: {0}", result);
            }
        }

        private static void Client()
        {
            Console.WriteLine("Client started");
            Thread.Sleep(1000);
            using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
            {
                Console.WriteLine("Connecting");
                pipe.Connect();

                Thread.Sleep(1000);
                Console.WriteLine("Writing byte");
                pipe.WriteByte(117);
            }
        }


        static void Main(string[] args)
        {
            //Console.WriteLine("Starting server");
            //var server = new Thread(Server);
            //server.Start();
            //
            //Console.WriteLine("Starting client");
            //var client = new Thread(Client);
            //client.Start();
            //
            //
            //client.Join();
            //server.Join();
            //Console.WriteLine("All done.");

            Server();
        }
    }
}
