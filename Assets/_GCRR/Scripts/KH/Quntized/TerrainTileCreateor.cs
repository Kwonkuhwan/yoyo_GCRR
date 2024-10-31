using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Tiles
{
    public static class TerrainTileCreateor
    {
        const float nTileSize = 32767.0f;

        public static Mesh CreateMesh(TerrainTileHeader headerData_, VertexData vertexData_, IndexData16 indexData16_, float fScale_, bool bReverse = false)
        {
            Mesh mesh = new Mesh();

            //각 점의 좌표. 3개의 값이 모여서 하나의 점(x,y,z)이 된다. 즉 반드시 3의 배수.
            double[] vertices = new double[vertexData_.vertexCount * 3];
            for (int i = 0; i < vertices.Length / 3; i++)
            {
                var temp = i * 3;
                vertices[temp] = vertexData_.u[i];
                vertices[temp + 1] = vertexData_.height[i];
                vertices[temp + 2] = vertexData_.v[i];
            }

            //폴리곤은 3각형으로 만들어진다. 각각은 위 vertices의 인덱스이며, 반드시 3의 배수여야 한다.
            int[] polygonVertexIndex = new int[indexData16_.indices.Length];

            // DH : 매쉬 반전용
            if (bReverse)
            {
                for (int i = 0; i < polygonVertexIndex.Length / 3; i++)
                {
                    var temp = i * 3;
                    polygonVertexIndex[temp] = indexData16_.indices[temp];
                    polygonVertexIndex[temp + 1] = indexData16_.indices[temp + 2];
                    polygonVertexIndex[temp + 2] = indexData16_.indices[temp + 1];
                }
            }
            else
            {
                for (int i = 0; i < polygonVertexIndex.Length; i++)
                {
                    polygonVertexIndex[i] = indexData16_.indices[i];
                }
            }

            List<int> trig = new List<int>();
            List<Vector3> vertex_ = new List<Vector3>();

            var temp_HeightScale = nTileSize / (headerData_.MaximumHeight - headerData_.MinimumHeight);
            for(int i = 0; i < vertices.Length / 3; i++)
            {
                var temp = i * 3;
                double x = vertices[temp] * fScale_;
                double y = (vertices[temp + 1] / temp_HeightScale) * fScale_ * 10;
                double z = vertices[temp + 2] * fScale_;

                vertex_.Add(new Vector3((float)x, (float)y, (float)z));
            }
                        
            for (int i = 0; i < polygonVertexIndex.Length; i++)
            {
                int poly = polygonVertexIndex[i];
                if(poly < 0)       //fbx 에서 음수 인덱스는 삼각형의 마지막 점을 나타낸다. *-1 하고 -1 하면 원래 값이 된다.
                {
                    poly = (poly * -1) - 1;
                }
                trig.Add(poly);
            }

            // _mesh 초기화
            mesh.Clear();
            //버텍스 데이터를 배열로 밀어 넣습니다.
            mesh.vertices = vertex_.ToArray();
            //인접한 버텍스 데이터를 배열로 밀어 넣습니다.
            mesh.triangles = trig.ToArray();

            mesh.uv = CalculateUv(vertexData_);

            return mesh;
        }


        static Vector2[] CalculateUv(VertexData vertexData_)
        {
            Vector2[] v2UVs = new Vector2[vertexData_.vertexCount];

            // DH : Defalut Quantized Mesh Vertics Range = (0,32767)
            for (int i = 0; i < vertexData_.vertexCount; i++)
            {
                v2UVs[i] = new Vector2(vertexData_.u[i] / nTileSize, vertexData_.v[i] / nTileSize);
            }

            return v2UVs;
        }
    }
}
