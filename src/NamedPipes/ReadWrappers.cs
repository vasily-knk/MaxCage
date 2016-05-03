using UnityEngine;

namespace NamedPipes
{
    partial class ReadWrappers
    {
        public static Vector2 ReadVector2(Bytes bytes)
        {
            Vector2 vec;
            vec.x = bytes.ReadFloat();
            vec.y = bytes.ReadFloat();
            return vec;
        }

        public static Vector3 ReadVector3(Bytes bytes)
        {
            Vector3 vec;
            vec.x = bytes.ReadFloat();
            vec.y = bytes.ReadFloat();
            vec.z = bytes.ReadFloat();
            return vec;
        }
        
        public static Quaternion ReadQuaternion(Bytes bytes)
        {
            Quaternion vec;
            vec.x = bytes.ReadFloat();
            vec.y = bytes.ReadFloat();
            vec.z = bytes.ReadFloat();
            vec.w = bytes.ReadFloat(); // !!!
            return vec;
        }

        public static Matrix4x4 ReadMatrix4x4(Bytes bytes)
        {
            Matrix4x4 m;
            m.m00 = bytes.ReadFloat();
            m.m01 = bytes.ReadFloat();
            m.m02 = bytes.ReadFloat();
            m.m03 = bytes.ReadFloat();
            m.m10 = bytes.ReadFloat();
            m.m11 = bytes.ReadFloat();
            m.m12 = bytes.ReadFloat();
            m.m13 = bytes.ReadFloat();
            m.m20 = bytes.ReadFloat();
            m.m21 = bytes.ReadFloat();
            m.m22 = bytes.ReadFloat();
            m.m23 = bytes.ReadFloat();
            m.m30 = bytes.ReadFloat();
            m.m31 = bytes.ReadFloat();
            m.m32 = bytes.ReadFloat();
            m.m33 = bytes.ReadFloat();
            return m;
        }
    }
}
