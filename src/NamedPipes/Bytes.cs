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

        public byte[] data { get; private set; }
        public int pos { get; set; } = 0;

        public int ReadInt32()
        {
            return BitConverter.ToInt32(data, advance(4));
        }

        public int ReadByte()
        {
            return data[advance(1)];
        }

        public string ReadString()
        {
            int len = ReadInt32();
            return Encoding.ASCII.GetString(data, advance(len), len);
        }

        private int advance(int offset)
        {
            int result = pos;
            pos += offset;
            return result;
        }
    }
}
