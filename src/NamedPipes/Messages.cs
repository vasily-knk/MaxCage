using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipes
{
    using EntityId = UInt32;

    class EntityData
        : IFromBytes
    {
        public string classname { get; private set; }
        public string name { get; private set; }

        public string getText()
        {
            return String.Format("{0} '{1}'", classname, name);
        }

        public void readFrom(Bytes bytes)
        {
            classname = bytes.ReadString();
            name = bytes.ReadString();
        }
    }

    class MsgAddEntity
        : IFromBytes
    {
        public EntityId id { get; private set; }
        public EntityData data { get; private set; }

        public void readFrom(Bytes bytes)
        {
            id = bytes.ReadUInt32();
            data = bytes.ReadGeneric<EntityData>();
        }
    }

    class MsgDeleteEntity
        : IFromBytes
    {
        public EntityId id { get; private set; }

        public void readFrom(Bytes bytes)
        {
            id = bytes.ReadUInt32();
        }
    }

    class MsgUpdateEntityTransform
        : IFromBytes
    {
        public EntityId id { get; private set; }

        public void readFrom(Bytes bytes)
        {
            id = bytes.ReadUInt32();
        }
    }

    class MsgNextFrame
        : IFromBytes
    {
        public void readFrom(Bytes bytes) { }
    }

}
