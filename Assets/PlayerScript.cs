using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private (long, long) chunkPos;
    public bool onSky = false;

    // Start is called before the first frame update
    void Start()
    {
        Globals.LoadResources();
        chunkPos = CalculateChunkPos();
        LoadVisibleChunks();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LoadVisibleChunks()
    {
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
            Thread thread = new Thread(() =>
            {
                Globals.ChunkGC(chunkPos);
                LoadVisibleChunks();
            });
            thread.Start();
        }

        while (!Globals.actionQueue.IsEmpty)
        {
            Globals.actionQueue.TryDequeue(out var action);
            action();
        }

        Vector3 translation = new Vector3();
        translation.x = Input.GetAxis("Horizontal");
        translation.z = Input.GetAxis("Vertical");
        transform.Translate(translation.normalized * Time.deltaTime * 5, Space.Self);

        Transform cameraTransform = Camera.main.transform;
        float rotation = (cameraTransform.eulerAngles.x <= 90 ? cameraTransform.eulerAngles.x : cameraTransform.eulerAngles.x - 360)
            - Input.GetAxis("Mouse Y") * Time.deltaTime * 50;
        cameraTransform.eulerAngles = new Vector3(
            Mathf.Clamp(rotation, -90, 90),
            cameraTransform.eulerAngles.y,
            cameraTransform.eulerAngles.z
            );
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * 80, 0), Space.Self);

        if (Input.GetAxis("Jump") != 0 && !onSky)
        {
            GetComponent<Rigidbody>().velocity += new Vector3(0, 5f, 0);
            onSky = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] points = new ContactPoint[collision.contactCount];
        collision.GetContacts(points);
        foreach (var i in points)
        {
            if (Mathf.Abs(i.normal.y) > 0.6f)
            {
                onSky = false;
                break;
            }
        }
    }
}
