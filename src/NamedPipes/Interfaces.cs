using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipes
{
    interface IBytesProcessor
    {
        void Process(int msgId, byte[] bytes);
    }

    interface IServer
    {
        void Start();
        bool isWorking();
    }

    interface IFromBytes
    {
        void readFrom(ref Bytes bytes);
    }
}
