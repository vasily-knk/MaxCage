using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipes
{
    class Bytes
    {
        public Bytes(byte[] data)
        {
            this.data = data;
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(data, advance(4));
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(data, advance(4));
        }

        public int ReadByte()
        {
            return data[advance(1)];
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(data, advance(4));
        }

        public string ReadString()
        {
            int len = ReadInt32();
            return Encoding.ASCII.GetString(data, advance(len), len);
        }

        public T ReadGeneric<T>()
            where T : IFromBytes, new()
        {
            T entry = new T();
            entry.readFrom(this);
            return entry;
        }

        public T[] ReadGenericArray<T>()
            where T : IFromBytes, new()
        {
            uint size = ReadUInt32();
            var data = new T[size];
            for (int i = 0; i < size; ++i)
                data[i] = ReadGeneric<T>();

            return data;
        }

        private int advance(int offset)
        {
            int result = pos;
            pos += offset;
            return result;
        }

        private byte[] data;
        private int pos = 0;
    }
}
