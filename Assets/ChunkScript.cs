using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Globals;

public class ChunkScript : MonoBehaviour
{
    ChunkMeshBuilder meshBuilder;
    GameObject player;

    public Chunk ChunkData
    {
        get;
        set;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Main Camera");
    }

    public void UpdateMesh()
    {
        if (ChunkData is null)
        {
            return;
        }

        if (player is null)
        {
            Start();
        }

        meshBuilder = new ChunkMeshBuilder();
        ChunkData.ForEach((x, y, z) =>
        {
            if (!ChunkData.Blocks[x, y, z].GetBlockType().Renderable)
            {
                return;
            }

            Vector3 vector = new Vector3(x, y, z);
            bool[] renderFace = new bool[6] { false, false, false, false, false, false };
            long index = GetChunkIndex(ChunkData.ChunkPos);

            // Top
            renderFace[0] = y == ChunkY - 1 || !ChunkData.Blocks[x, y + 1, z].GetBlockType().Solid;

            // Front
            if (z != 0)
            {
                renderFace[1] = !ChunkData.Blocks[x, y, z - 1].GetBlockType().Solid;
            }
            else if (Chunks.ContainsKey(index - MaxChunkCount))
            {
                renderFace[1] = !Chunks[index - MaxChunkCount].Blocks[x, y, ChunkZ - 1].GetBlockType().Solid;
            }

            // Left
            if (x != 0)
            {
                renderFace[2] = !ChunkData.Blocks[x - 1, y, z].GetBlockType().Solid;
            }
            else if (Chunks.ContainsKey(index - 1))
            {
                renderFace[2] = !Chunks[index - 1].Blocks[ChunkX - 1, y, z].GetBlockType().Solid;
            }

            // Right
            if (x != ChunkX - 1)
            {
                renderFace[3] = !ChunkData.Blocks[x + 1, y, z].GetBlockType().Solid;
            }
            else if (Chunks.ContainsKey(index + 1))
            {
                renderFace[3] = !Chunks[index + 1].Blocks[0, y, z].GetBlockType().Solid;
            }

            // Back
            if (z != ChunkZ - 1)
            {
                renderFace[4] = !ChunkData.Blocks[x, y, z + 1].GetBlockType().Solid;
            }
            else if (Chunks.ContainsKey(index + MaxChunkCount))
            {
                renderFace[4] = !Chunks[index + MaxChunkCount].Blocks[x, y, 0].GetBlockType().Solid;
            }

            // Bottom
            renderFace[5] = y == 0 || !ChunkData.Blocks[x, y - 1, z].GetBlockType().Solid;


            for (int i = 0; i < 6; i++)
            {
                if (renderFace[i])
                {
                    meshBuilder.AddFace(i, vector, ChunkData.Blocks[x, y, z]);
                }
            }
        });

        Mesh mesh = meshBuilder.BuildMesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().material = PackedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateMesh();
    }
}
