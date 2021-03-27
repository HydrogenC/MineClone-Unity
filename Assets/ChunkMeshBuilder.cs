using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ChunkMeshBuilder
{
    private List<Vector3> vertices;
    private List<int[]> submeshes;
    private List<Vector2> uvs;

    public ChunkMeshBuilder()
    {
        vertices = new List<Vector3>();
        submeshes = new List<int[]>();
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

    public void AddSubmesh(int[] triangles)
    {
        submeshes.Add(triangles);
    }

    public Mesh BuildMesh()
    {
        Mesh mesh = new Mesh();
        mesh.subMeshCount = submeshes.Count;
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        for (int i=0;i<submeshes.Count;i++)
        {
            mesh.SetTriangles(submeshes[i], i);
        }
        mesh.RecalculateNormals();
        return mesh;
    }

    public int SubMeshCount => submeshes.Count;
}
