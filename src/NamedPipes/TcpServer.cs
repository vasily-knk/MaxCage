using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace NamedPipes
{
    class TcpServer
        : IServer
        , IDisposable
    {
        private NetworkStream pipe_;
        private IBytesProcessor processor_;

        private class ReadBuffer
        {
            public byte[] data { get; private set; }
            public int pos { get; set; }

            public ReadBuffer(int size)
            {
                data = new byte[size];
                pos = 0;
            }
        }


        private class MessageData
        {
            public byte[] buffer;
            public int msgId;
        }


        public TcpServer(IBytesProcessor processor)
        {
            processor_ = processor;
        }

        public void Start()
        {
            var listener = new TcpListener(45115);
            listener.Start();
            listener.BeginAcceptTcpClient(OnAccept, listener);
        }

        private void OnAccept(IAsyncResult result)
        {
            var listener = (TcpListener)result.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(result);
            Console.WriteLine("Client connected");
            pipe_ = client.GetStream();
            Ready();
        }


        private delegate void AllReadCallback(object arg);
        private delegate void DisconnectCallback();

        private class ReadData
        {
            public ReadBuffer buffer;
            public AllReadCallback callback;
            public object arg;
            public DisconnectCallback disconnectCallback;
        }

        private void ReadAll<T>(ReadBuffer buffer, AllReadCallback callback, T arg, DisconnectCallback disconnectCallback)
        {
            var data = new ReadData {
                buffer = buffer,
                callback = callback,
                arg = arg,
                disconnectCallback = disconnectCallback
            };

            pipe_.BeginRead(buffer.data, 0, buffer.data.Length, OnChunk, data);
        }

        private void OnChunk(IAsyncResult result)
        {
            var data = (ReadData)result.AsyncState;
            int count = pipe_.EndRead(result);

            //Console.WriteLine("Got {0}, expected {1}", count, data.buffer.data.Length - data.buffer.pos);

            if (count == 0)
            {
                data.disconnectCallback();
                return;
            }

            data.buffer.pos += count;

            if (data.buffer.pos < data.buffer.data.Length)
            {
                pipe_.BeginRead(data.buffer.data, data.buffer.pos, data.buffer.data.Length - data.buffer.pos, OnChunk, data);
                return;
            }

            data.callback(data.arg);
        }

        private void Ready()
        {
            var buffer = new ReadBuffer(12);
            ReadAll<byte[]>(buffer, OnHeader, buffer.data, Disconnect);
        }

        private void OnHeader(object arg)
        {
            var hdrBuffer = (byte[])arg;
            var hdrBytes = new Bytes(hdrBuffer);

            uint magic = hdrBytes.ReadUInt32();
            if (magic != 117)
            {
                UnityEngine.Debug.LogErrorFormat("Invalid magic: {0} !!!", magic);
                return;
            }

            uint msgLength = hdrBytes.ReadUInt32();
            int msgId = hdrBytes.ReadInt32();
            var msgBuffer = new ReadBuffer((int)msgLength);
            var msgData = new MessageData { buffer = msgBuffer.data, msgId = msgId };

            if (msgLength == 0)
            {
                OnMessage(msgData);
                return;
            }


            ReadAll<MessageData>(msgBuffer, OnMessage, msgData, Disconnect);
        }

        private void OnMessage(object arg)
        {
            var msgData = (MessageData)arg;


            processor_.Process(msgData.msgId, msgData.buffer);
            Ready();
        }

        private void Disconnect()
        {
            Console.WriteLine("Client disconnected");
            Dispose();
        }

        public bool isWorking()
        {
            return true;
        }

        public void Dispose()
        {
        }

    }
}
