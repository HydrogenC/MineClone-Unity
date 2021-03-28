using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk
{
    public bool Loaded
    {
        get;
        set;
    } = false;

    public bool ForceLoad
    {
        get;
        set;
    } = false;

    public bool Generated
    {
        get;
        set;
    } = false;

    public (long, long) ChunkPos
    {
        get;
        set;
    }

    public IBlockState[,,] Blocks
    {
        get;
        set;
    } = new IBlockState[16, 512, 16];

    public Chunk(long chunkX, long chunkZ)
    {
        ChunkPos = (chunkX, chunkZ);
        int[,] perlin = new int[Globals.ChunkX, Globals.ChunkZ];
        for (int x = 0; x < Globals.ChunkX; x++)
        {
            for (int z = 0; z < Globals.ChunkZ; z++)
            {
                perlin[x, z] = 64 + Mathf.RoundToInt(6 * Mathf.PerlinNoise((ChunkPos.Item1 * Globals.ChunkX + x) / 16.789f, (ChunkPos.Item2 * Globals.ChunkZ + z) / 16.789f));
            }
        }

        ForEach((x, y, z) =>
        {
            if (y < perlin[x, z])
            {
                Blocks[x, y, z] = Globals.BlockTypes[1].GetDefaultBlockState(Facing.PosY);
            }
            else if (y == perlin[x, z])
            {
                Blocks[x, y, z] = Globals.BlockTypes[2].GetDefaultBlockState(Facing.PosY);
            }
            else
            {
                Blocks[x, y, z] = Globals.BlockTypes[0].GetDefaultBlockState(Facing.PosY);
            }
        });
    }

    public void ForEach(Action<int, int, int> action)
    {
        for (int x = 0; x < Globals.ChunkX; x++)
        {
            for (int y = 0; y < Globals.ChunkY; y++)
            {
                for (int z = 0; z < Globals.ChunkZ; z++)
                {
                    action(x, y, z);
                }
            }
        }
    }
}