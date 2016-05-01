using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipes
{
    public class UnknownMessageId
        : Exception
    {
        public UnknownMessageId(int id)
        {
            this.id = id;
        }

        public int id { get; private set; }
    }

    class Dispatcher
        : IBytesProcessor
    {
        public delegate void StructProcessor<T>(T data);

        public void Add<T>(int msgId, StructProcessor<T> structProc)
            where T : IFromBytes, new()
        {
            BytesProcessor f = bytes => structProc(Convert<T>(bytes));
            processors_.Add(msgId, f);
        }

        public void Process(int msgId, byte[] bytes)
        {
            BytesProcessor proc;
            try
            {
                proc = processors_[msgId]; 
            }
            catch (KeyNotFoundException)
            {
                throw new UnknownMessageId(msgId);
            }

            proc(bytes);
        }

        private static T Convert<T>(byte[] bytes) 
            where T : IFromBytes, new()
        {
            var value = new T();

            var bytesWrapper = new Bytes(bytes);
            value.readFrom(ref bytesWrapper);
            return value;
        }

        private delegate void BytesProcessor(byte[] bytes);
        private Dictionary<int, BytesProcessor> processors_ = new Dictionary<int, BytesProcessor>();
    }
}
