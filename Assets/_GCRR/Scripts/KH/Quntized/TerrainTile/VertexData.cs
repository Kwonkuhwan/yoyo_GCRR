using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Terrain.Tiles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexData
    {
        public uint vertexCount;
        public ushort[] u;
        public ushort[] v;
        public ushort[] height;

        public VertexData(BinaryReader reader)
        {
            vertexCount = reader.ReadUInt32();

            u = new ushort[vertexCount];
            v = new ushort[vertexCount];
            height = new ushort[vertexCount];

            if (vertexCount > 64 * 1024)
            {
                return;
                throw new NotSupportedException("32 bit indices not supported yet");
            }

            for (int i = 0; i < vertexCount; i++)
                u[i] = reader.ReadUInt16();

            for (int i = 0; i < vertexCount; i++)
                v[i] = reader.ReadUInt16();

            for (int i = 0; i < vertexCount; i++)
                height[i] = reader.ReadUInt16();

            ushort u_ = 0;
            ushort v_ = 0;
            ushort height_ = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                u_ += (ushort)ZigZag.Decode(u[i]);
                v_ += (ushort)ZigZag.Decode(v[i]);
                height_ += (ushort)ZigZag.Decode(height[i]);

                u[i] = u_;
                v[i] = v_;
                height[i] = height_;
            }
        }
    }
}
