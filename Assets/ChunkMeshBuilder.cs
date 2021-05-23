using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Globals;

class ChunkMeshBuilder
{
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;
    private int faceCount = 0;

    public ChunkMeshBuilder()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
    }

    public void AddVertices(ICollection<Vector3> vertices, ICollection<Vector2> uvs)
    {
        this.vertices.AddRange(vertices);
        this.uvs.AddRange(uvs);
    }

    public void AddVertice(Vector3 vertices, Vector2 uvs)
    {
        this.vertices.Add(vertices);
        this.uvs.Add(uvs);
    }

    public void AddTriangles(int[] newTriangles)
    {
        triangles.AddRange(newTriangles);
    }

    public void AddFace(int faceIndex, Vector3 basePos, IBlockState block)
    {
        Rect uvRect = block.GetFaceUVRect(faceIndex);
        Vector2[] uvCoords = new Vector2[]
        {
            new Vector2(uvRect.xMin, uvRect.yMin),
            new Vector2(uvRect.xMin, uvRect.yMax),
            new Vector2(uvRect.xMax, uvRect.yMin),
            new Vector2(uvRect.xMax, uvRect.yMax)
        };
        for (int i = 0; i < Vertices[faceIndex].Length; i++)
        {
            AddVertice(Vertices[faceIndex][i] + basePos, uvCoords[i]);
        }

        int[] triangles = new int[Triangles.Length];
        for (int i = 0; i < Triangles.Length; i++)
        {
            triangles[i] = Triangles[i] + FaceCount * 4;
        }
        AddTriangles(triangles);
        faceCount++;
    }

    public Mesh BuildMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            uv = uvs.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.RecalculateNormals();
        return mesh;
    }

    public int FaceCount => faceCount;
}
