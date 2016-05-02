using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipes
{
    using EntityId = UInt32;
    using ModelId = UInt32;

    class Matrix4
        : IFromBytes
    {
        public float[] data { get; set; } = new float[16];

        public void readFrom(Bytes bytes)
        {
            for (int i = 0; i < data.Length; ++i)
                data[i] = bytes.ReadFloat();
        }
    }

    struct Vector2
        : IFromBytes
    {
        public float x, y;
        public void readFrom(Bytes bytes)
        {
            x = bytes.ReadFloat();
            y = bytes.ReadFloat();
        }
    }

    struct Vector3
        : IFromBytes
    {
        public float x, y, z;
        public void readFrom(Bytes bytes)
        {
            x = bytes.ReadFloat();
            y = bytes.ReadFloat();
            z = bytes.ReadFloat();
        }
    }

    struct Vertex
        : IFromBytes
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2[] uvs;

        public void readFrom(Bytes bytes)
        {
            pos = bytes.ReadGeneric<Vector3>();
            normal = bytes.ReadGeneric<Vector3>();
            uvs = bytes.ReadGenericArray<Vector2>();
        }
    }

    struct Mesh
        : IFromBytes
    {
        public int[] indices;

        public void readFrom(Bytes bytes)
        {
            indices = bytes.ReadInt32Array();
        }
    }

    class EntityData
        : IFromBytes
    {
        public string classname { get; private set; }
        public string name { get; private set; }
        public Matrix4 transform { get; set; }

        public string getText()
        {
            return String.Format("{0} '{1}'", classname, name);
        }

        public void readFrom(Bytes bytes)
        {
            classname = bytes.ReadString();
            name = bytes.ReadString();
            transform = bytes.ReadGeneric<Matrix4>();
        }
    }

    class Model
        : IFromBytes
    {
        public string name { get; private set; }
        public Vertex[] verts { get; private set; }
        public Mesh[] meshes { get; private set; }

        public void readFrom(Bytes bytes)
        {
            name = bytes.ReadString();
            verts = bytes.ReadGenericArray<Vertex>();
            meshes = bytes.ReadGenericArray<Mesh>();
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
        public Matrix4 transform { get; private set; }

        public void readFrom(Bytes bytes)
        {
            id = bytes.ReadUInt32();
            transform = bytes.ReadGeneric<Matrix4>();
        }
    }

    class MsgAddModel
        : IFromBytes
    {
        public ModelId id { get; private set; }
        public Model model { get; private set; }

        public void readFrom(Bytes bytes)
        {
            id = bytes.ReadUInt32();
            model = bytes.ReadGeneric<Model>();
        }
    }

    class MsgNextFrame
        : IFromBytes
    {
        public void readFrom(Bytes bytes) { }
    }

}
