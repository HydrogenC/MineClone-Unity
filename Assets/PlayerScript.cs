using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    const float horizentalRotationSpeed = 120, verticalRotationSpeed = 75;
    private (long, long) chunkPos = (long.MaxValue, long.MaxValue);
    public bool onSky = false, locked = true;
    public GameObject wireFrame;
    float ty = 0;

    // Start is called before the first frame update
    void Start()
    {
        Globals.LoadResources();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ty = transform.position.y;
    }

    static void LoadVisibleChunks(long x, long z)
    {
        for (long i = x - Globals.RenderDistance;
            i <= x + Globals.RenderDistance;
            i++)
        {
            for (long j = z - Globals.RenderDistance;
            j <= z + Globals.RenderDistance;
            j++)
            {
                Globals.GenerateChunk(i, j);
            }
        }

        for (long i = x - Globals.RenderDistance;
            i <= x + Globals.RenderDistance;
            i++)
        {
            for (long j = z - Globals.RenderDistance;
            j <= z + Globals.RenderDistance;
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
            if (!Globals.Chunks.ContainsKey(Globals.GetChunkIndex(newChunkPos)))
            {
                GetComponent<Rigidbody>().useGravity = false;
                locked = true;
            }

            chunkPos = newChunkPos;
            Thread thread = new Thread(() =>
            {
                Globals.ChunkGC(chunkPos);
                LoadVisibleChunks(chunkPos.Item1, chunkPos.Item2);
            });
            thread.Start();
        }

        while (!Globals.ActionQueue.IsEmpty)
        {
            Globals.ActionQueue.TryDequeue(out var action);
            action();
        }

        if (locked)
        {
            if (locked = !Globals.Chunks.ContainsKey(Globals.GetChunkIndex(newChunkPos)))
            {
                return;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, ty, transform.position.z);
            }
        }

        GetComponent<Rigidbody>().useGravity = true;
        Vector3 translation = new Vector3();
        translation.x = Input.GetAxis("Horizontal");
        translation.z = Input.GetAxis("Vertical");
        transform.Translate(translation.normalized * Time.deltaTime * 5, Space.Self);

        Transform cameraTransform = Camera.main.transform;
        float cameraRotation = (cameraTransform.eulerAngles.x <= 90 ? cameraTransform.eulerAngles.x : cameraTransform.eulerAngles.x - 360)
            - Input.GetAxis("Mouse Y") * Time.deltaTime * verticalRotationSpeed;
        cameraTransform.eulerAngles = new Vector3(
            Mathf.Clamp(cameraRotation, -90, 90),
            cameraTransform.eulerAngles.y,
            cameraTransform.eulerAngles.z
            );
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * horizentalRotationSpeed, 0), Space.Self);

        if (Input.GetAxis("Jump") != 0 && !onSky)
        {
            GetComponent<Rigidbody>().velocity += new Vector3(0, 5f, 0);
            onSky = true;
        }

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hitInfo, 10))
        {
            //Debug.Log("Raycasted at" + hitInfo.point.ToString());
            BlockPos pos = new BlockPos(Mathf.FloorToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y - 1), Mathf.FloorToInt(hitInfo.point.z));
            wireFrame.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z + 0.5f);
            wireFrame.GetComponent<MeshRenderer>().enabled = true;

            if (Input.GetMouseButtonDown(0))
            {
                Globals.SetBlockAtPos(pos, Globals.BlockTypes[0].GetDefaultBlockState(Facing.PosY));
            }
        }
        else
        {
            wireFrame.GetComponent<MeshRenderer>().enabled = false;
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
