using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private (long, long) chunkPos;

    // Start is called before the first frame update
    void Start()
    {
        Globals.LoadResources();
        chunkPos = CalculateChunkPos();
        for (long i = chunkPos.Item1 - Globals.RenderDistance;
            i <= chunkPos.Item1 + Globals.RenderDistance;
            i++)
        {
            for (long j = chunkPos.Item2 - Globals.RenderDistance;
            j <= chunkPos.Item2 + Globals.RenderDistance;
            j++)
            {
                Globals.GenerateChunk(i, j);
            }
        }

        for (long i = chunkPos.Item1 - Globals.RenderDistance;
            i <= chunkPos.Item1 + Globals.RenderDistance;
            i++)
        {
            for (long j = chunkPos.Item2 - Globals.RenderDistance;
            j <= chunkPos.Item2 + Globals.RenderDistance;
            j++)
            {
                Globals.LoadChunk(i, j);
            }
        }
    }

    (long, long) CalculateChunkPos()
    {
        return (
            Mathf.FloorToInt(transform.position.x / 16),
            Mathf.FloorToInt(transform.position.z / 16)
            );
    }

    // Update is called once per frame
    void Update()
    {
        (long, long) newChunkPos = CalculateChunkPos();

        if (newChunkPos != chunkPos)
        {
            chunkPos = newChunkPos;
            Globals.ChunkGC(chunkPos);
        }

    }
}
