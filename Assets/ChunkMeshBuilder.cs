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
