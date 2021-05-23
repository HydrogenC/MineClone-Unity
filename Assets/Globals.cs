using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public struct BlockPos
{
    public long x, y, z;

    public BlockPos(long x, long y, long z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public BlockPos(Vector3 pos)
    {
        x = Mathf.FloorToInt(pos.x);
        y = Mathf.FloorToInt(pos.y);
        z = Mathf.FloorToInt(pos.z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

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

    public static readonly Vector2[] DefaultColorMapUV =
    {
        new Vector2(1,1),
        new Vector2(1,1),
        new Vector2(1,1),
        new Vector2(1,1)
    };

    public static readonly Vector2[] GrassColorMapUV =
    {
        new Vector2(0.4f,0.4f),
        new Vector2(0.4f,0.4f),
        new Vector2(0.4f,0.4f),
        new Vector2(0.4f,0.4f)
    };

    public const int ChunkX = 16;
    public const int ChunkY = 512;
    public const int ChunkZ = 16;
    public const long MaxChunkCount = 500000;

    public static int RenderDistance
    {
        get;
        set;
    } = 3;

    public static ConcurrentQueue<Action> ActionQueue = new ConcurrentQueue<Action>();
    public static ConcurrentDictionary<long, Chunk> Chunks = new ConcurrentDictionary<long, Chunk>();
    public static Dictionary<string, Rect> Textures = new Dictionary<string, Rect>();
    public static Block[] BlockTypes = new Block[]
    {
        new Block
        {
            BlockId="air",
            Solid = false,
            Renderable = false,
            DefaultBlockState=new EmptyBlockState()
        },
        new Block
        {
            BlockId="dirt",
            DefaultBlockState = new BasicBlockState(
                new string[]
                {
                    "dirt",
                    "dirt",
                    "dirt",
                    "dirt",
                    "dirt",
                    "dirt"
                }
                )
        },
        new Block
        {
            BlockId="grass",
            DefaultBlockState = new GrassBlockState()
        }
    };

    public static GameObject ChunkObject;
    public static Material PackedMaterial;

    static Texture2D ChangeFormat(this Texture2D oldTexture, TextureFormat newFormat)
    {
        //Create new empty Texture
        Texture2D newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, false);
        //Copy old texture pixels into new one
        newTex.SetPixels(oldTexture.GetPixels());
        //Apply
        newTex.Apply();

        return newTex;
    }

    public static void LoadResources()
    {
        ChunkObject = Resources.Load<GameObject>("Chunk");
        PackedMaterial = Resources.Load<Material>("SolidMaterial");
        var n = Resources.Load<TextAsset>("Blocks/Materials");
        XmlDocument document = new XmlDocument();
        document.LoadXml(n.text);

        List<Texture2D> textures = new List<Texture2D>();
        foreach (XmlElement i in document.DocumentElement.ChildNodes)
        {
            textures.Add(Resources.Load<Texture2D>("Textures/" + i.GetAttribute("texture")));
        }
        Texture2D packedTexture = new Texture2D(8192, 8192);
        var rects = packedTexture.PackTextures(textures.ToArray(), 4);
        packedTexture = packedTexture.ChangeFormat(TextureFormat.RGBA32);
        uPaddingBleed.BleedEdges(packedTexture, 4, rects, false);
        packedTexture.filterMode = FilterMode.Point;
        int index = 0;
        foreach (var rect in rects)
        {
            Textures.Add((document.DocumentElement.ChildNodes[index] as XmlElement).GetAttribute("name"), rect);
            index++;
        }

        PackedMaterial.SetTexture("_MainTex", packedTexture);

        foreach (var i in BlockTypes)
        {
            if (i.DefaultBlockState is BasicBlockState state)
            {
                state.SetBlockType(i);
            }
        }
    }

    public static IBlockState GetBlockAtPos(BlockPos pos)
    {
        return Chunks[GetChunkIndex(pos.x / ChunkX, pos.z / ChunkZ)].Blocks[(pos.x + ChunkX) % ChunkX, pos.y, (pos.z + ChunkZ) % ChunkZ];
    }

    public static void SetBlockAtPos(BlockPos pos, IBlockState state)
    {
        Chunk chunk = Chunks[GetChunkIndex(Mathf.FloorToInt((float)pos.x / ChunkX), Mathf.FloorToInt((float)pos.z / ChunkZ))];
        chunk.Blocks[(pos.x + ChunkX) % ChunkX, pos.y, (pos.z + ChunkZ) % ChunkZ] = state;
        GameObject.Find($"Chunk {chunk.ChunkPos.Item1} {chunk.ChunkPos.Item2}").GetComponent<ChunkScript>().UpdateMesh();
        Debug.Log($"Chunk {chunk.ChunkPos.Item1} {chunk.ChunkPos.Item2} At {(pos.x + ChunkX) % ChunkX} {pos.y} {(pos.z + ChunkZ) % ChunkZ}");
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
        ActionQueue.Enqueue(() =>
        {
            GameObject obj = GameObject.Instantiate(ChunkObject);
            obj.name = $"Chunk {x} {z}";
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

            if (Math.Abs(x - playerPos.Item1) > RenderDistance || Math.Abs(z - playerPos.Item2) > RenderDistance)
            {
                ActionQueue.Enqueue(() => GameObject.Destroy(GameObject.Find($"Chunk {x} {z}")));
                i.Value.Loaded = false;
                count++;
            }
        }

        Debug.Log($"Removed {count} chunks! ");
    }
}
