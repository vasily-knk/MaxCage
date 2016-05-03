using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NamedPipes
{
    using EntityId = UInt32;
    using ModelId = UInt32;

    struct Vertex
        : IFromBytes
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2[] uvs;

        public void readFrom(Bytes bytes)
        {
            pos = ReadWrappers.ReadVector3(bytes);
            normal = ReadWrappers.ReadVector3(bytes);
            uvs = bytes.ReadArray<Vector2>(ReadWrappers.ReadVector2);
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

    struct Transform
        : IFromBytes
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        
        public void readFrom(Bytes bytes)
        {
            pos = ReadWrappers.ReadVector3(bytes);
            rot = ReadWrappers.ReadQuaternion(bytes);
            scale = ReadWrappers.ReadVector3(bytes);
        }
    }

    class EntityData
        : IFromBytes
    {
        public string classname { get; private set; }
        public string name { get; private set; }
        public Transform transform { get; set; }
        public ModelId modelId { get; set; }

        public string getText()
        {
            return String.Format("{0} '{1}'", classname, name);
        }

        public void readFrom(Bytes bytes)
        {
            classname = bytes.ReadString();
            name = bytes.ReadString();
            transform = bytes.Read<Transform>();
            modelId = bytes.ReadUInt32();
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
            verts = bytes.ReadArray<Vertex>();
            meshes = bytes.ReadArray<Mesh>();
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
            data = bytes.Read<EntityData>();
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
        public Transform transform { get; private set; }

        public void readFrom(Bytes bytes)
        {
            id = bytes.ReadUInt32();
            transform = bytes.Read<Transform>();
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
            model = bytes.Read<Model>();
        }
    }

    class MsgNextFrame
        : IFromBytes
    {
        public void readFrom(Bytes bytes) { }
    }

}
