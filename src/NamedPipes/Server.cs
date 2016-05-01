using System;
using System.Text;
using System.IO.Pipes;


namespace NamedPipes
{
    class Server
        : IDisposable
    {
        private NamedPipeServerStream pipe_;

        public bool isWorking { get; private set; } = true;

        public Server(string pipeName)
        {
            pipe_ = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            Console.WriteLine("Waiting for connection");
            pipe_.BeginWaitForConnection(this.OnConnection, null);
        }

        public void Dispose()
        {
            pipe_.Close();
        }

        private void OnConnection(IAsyncResult result)
        {
            pipe_.EndWaitForConnection(result);
            Console.WriteLine("Reading bytes");
            Ready();
        }

        private void Ready()
        {
            var buffer = new byte[4];
            pipe_.BeginRead(buffer, 0, buffer.Length, OnLength, buffer);
        }

        private void OnLength(IAsyncResult result)
        {
            int count = pipe_.EndRead(result);
            var sizeBuffer = (byte[])result.AsyncState;

            uint msgLength = BitConverter.ToUInt32(sizeBuffer, 0);

            if (msgLength == 0)
            {
                isWorking = false;
                return;
            }

            var msgBuffer = new byte[msgLength];
            pipe_.BeginRead(msgBuffer, 0, (int)msgLength, OnMessage, msgBuffer);
        }

        private void OnMessage(IAsyncResult result)
        {
            int count = pipe_.EndRead(result);
            var msgBuffer = (byte[])result.AsyncState;

            string text = Encoding.ASCII.GetString(msgBuffer);

            Console.WriteLine("Message: {0}", text);

            Ready();
        }
    }
}
