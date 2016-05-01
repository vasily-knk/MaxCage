using System;
using System.Text;
using System.IO.Pipes;


namespace NamedPipes
{
    class Server
        : IDisposable
        , IServer
    {
        private string pipeName_;
        private NamedPipeServerStream pipe_;
        private IBytesProcessor processor_;

        public bool isWorking()
        {
            return pipe_ != null;
        }

        public Server(string pipeName, IBytesProcessor processor)
        {
            pipeName_ = pipeName;
            processor_ = processor;
        }

        public void Start()
        {
            pipe_ = new NamedPipeServerStream(pipeName_, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            Console.WriteLine("Waiting for connection");
            pipe_.BeginWaitForConnection(this.OnConnection, null);
        }

        public void Dispose()
        {
            if (pipe_ != null)
                pipe_.Close();
            pipe_ = null;
        }

        private void OnConnection(IAsyncResult result)
        {
            pipe_.EndWaitForConnection(result);
            Console.WriteLine("Reading bytes");
            Ready();
        }

        private void Ready()
        {
            var buffer = new byte[8];
            pipe_.BeginRead(buffer, 0, buffer.Length, OnHeader, buffer);
        }

        private bool ProcessCount(int count, int expected)
        {
            if (count == 0)
            {
                Dispose();
                return false;
            }

            return true;
        }

        private class MessageData
        {
            public byte[] buffer;
            public uint length;
            public int msgId;
        }

        private void OnHeader(IAsyncResult result)
        {
            if (!ProcessCount(pipe_.EndRead(result), 8))
                return;

            var hdrBuffer = (byte[])result.AsyncState;
            int msgId = BitConverter.ToInt32(hdrBuffer, 0);
            uint msgLength = BitConverter.ToUInt32(hdrBuffer, 4);

            var msgData = new MessageData { buffer = new byte[msgLength], length = msgLength, msgId = msgId };
            pipe_.BeginRead(msgData.buffer, 0, (int)msgLength, OnMessage, msgData);
        }

        private void OnMessage(IAsyncResult result)
        {
            var msgData = (MessageData)result.AsyncState;

            processor_.Process(msgData.msgId, msgData.buffer);
            Ready();
        }
    }
}
