using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        void readFrom(Bytes bytes);
    }
}
