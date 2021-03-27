using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public static class Globals
{
    public static readonly Vector3[][] Vertices =
    {
        // Top
        new Vector3[]
        {
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1,1,0),
            new Vector3(1,1,1)
        },
        // Front
        new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0)
        },
        // Left
        new Vector3[]
        {
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(0,0,0),
            new Vector3(0,1,0)
        },
        // Right
        new Vector3[]
        {
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(1,0,1),
            new Vector3(1,1,1)
        },
        // Back
        new Vector3[]
        {
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,0,1),
            new Vector3(0,1,1)
        },
        // Bottom
        new Vector3[]
        {
            new Vector3(0,0,1),
            new Vector3(0,0,0),
            new Vector3(1,0,1),
            new Vector3(1,0,0)
        }
    };

    public static readonly int[] Triangles =
    {
        0,1,2,2,1,3
    };

    public static readonly Vector2[] UVs =
    {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1)
    };

    public const int ChunkX = 16;
    public const int ChunkY = 512;
    public const int ChunkZ = 16;
    public const long MaxChunkCount = 500000;

    public static int RenderDistance
    {
        get;
        set;
    } = 5;

    public static ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
    public static ConcurrentDictionary<long, Chunk> Chunks = new ConcurrentDictionary<long, Chunk>();
    public static Dictionary<string, Material> Materials = new Dictionary<string, Material>();
    public static Block[] Blocks = new Block[]
    {
        new Block
        {
            BlockId="air",
            Solid = false,
            Renderable = false
        },
        new Block
        {
            BlockId="dirt",
            BaseMaterialNames = new string[]
            {
                "dirt",
                "dirt",
                "dirt",
                "dirt",
                "dirt",
                "dirt"
            }
        },
        new Block
        {
            BlockId="grass",
            BaseMaterialNames = new string[]
            {
                "grass_top",
                "grass_side",
                "grass_side",
                "grass_side",
                "grass_side",
                "dirt"
            }
        }
    };

    public static GameObject chunkObject;
    public static Material solidMaterial;

    public static void LoadResources()
    {
        chunkObject = Resources.Load<GameObject>("Chunk");
        solidMaterial = Resources.Load<Material>("SolidMaterial");
        var n = Resources.Load<TextAsset>("Blocks/Materials");
        XmlDocument document = new XmlDocument();
        document.LoadXml(n.text);

        foreach (XmlElement i in document.DocumentElement.ChildNodes)
        {
            Material material = Material.Instantiate(solidMaterial);
            material.mainTexture = Resources.Load<Texture>("Textures/" + i.GetAttribute("texture"));
            Materials.Add(i.GetAttribute("name"), material);
        }
    }

    public static long GetChunkIndex(long x, long z)
    {
        return z * MaxChunkCount + x;
    }

    public static long GetChunkIndex((long, long) pos)
    {
        return pos.Item2 * MaxChunkCount + pos.Item1;
    }

    public static void GenerateChunk(long x, long z)
    {
        long index = GetChunkIndex(x, z);
        if (!Chunks.ContainsKey(index))
        {
            Chunks[index] = new Chunk(x, z);
        }
    }

    public static void LoadChunk(long x, long z)
    {
        long index = GetChunkIndex(x, z);
        if (Chunks[index].Loaded)
        {
            return;
        }

        Chunks[index].Loaded = true;
        actionQueue.Enqueue(() =>
        {
            Chunks[index].ProcessGrass();
            GameObject obj = GameObject.Instantiate(chunkObject);
            obj.transform.position = new Vector3(x * ChunkX, 0, z * ChunkZ);
            ChunkScript cs = obj.AddComponent<ChunkScript>();
            cs.ChunkData = Chunks[index];
            cs.UpdateMesh();
        });
    }

    public static void ChunkGC((long, long) playerPos)
    {
        int count = 0;
        foreach (var i in Chunks)
        {
            if (i.Value.ForceLoad)
            {
                continue;
            }
            long x = i.Key % MaxChunkCount, z = i.Key / MaxChunkCount;

            if (Math.Abs(x - playerPos.Item1) > RenderDistance && Math.Abs(z - playerPos.Item2) > RenderDistance)
            {
                actionQueue.Enqueue(() => GameObject.Destroy(GameObject.Find($"Chunk {x} {z}")));
                i.Value.Loaded = false;
                count++;
            }
        }

        Debug.Log($"Removed {count} chunks! ");
    }
}
