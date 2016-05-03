using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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


        public T Read<T>()
            where T : IFromBytes, new()
        {
            T entry = new T();
            entry.readFrom(this);
            return entry;
        }

        public static T Read<T>(Bytes bytes)
            where T : IFromBytes, new()
        {
            return bytes.Read<T>();
        }

        public T[] ReadArray<T>()
            where T : IFromBytes, new()
        {
            ReadFunc<T> f = Read<T>;
            return ReadArray<T>(f);
        }

        public int[] ReadInt32Array()
        {
            var data = CreateArray<int>();
            for (int i = 0; i < data.Length; ++i)
                data[i] = ReadInt32();

            return data;
        }

        public delegate T ReadFunc<T>(Bytes bytes);
        
        public T[] ReadArray<T>(ReadFunc<T> func)
        {
            var data = CreateArray<T>();
            for (int i = 0; i < data.Length; ++i)
                data[i] = func(this);

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
