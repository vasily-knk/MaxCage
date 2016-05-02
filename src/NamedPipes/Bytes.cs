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
            var data = CreateArray<T>();
            for (int i = 0; i < data.Length; ++i)
                data[i] = ReadGeneric<T>();

            return data;
        }

        public int[] ReadInt32Array()
        {
            var data = CreateArray<int>();
            for (int i = 0; i < data.Length; ++i)
                data[i] = ReadInt32();

            return data;
        }

        private T[] CreateArray<T>()
        {
            uint size = ReadUInt32();
            return new T[size];
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
