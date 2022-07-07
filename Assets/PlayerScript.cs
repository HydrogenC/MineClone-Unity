using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    const float cameraSmoothing = 20.0f;
    private (long, long) chunkPos = (long.MaxValue, long.MaxValue);
    public bool onSky = false, locked = true;
    public GameObject wireFrame;
    float ty = 0;

    private float pitch;
    private float yaw;

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

        Transform cameraTransform = Camera.main.transform;
        Vector3 translation = new Vector3();
        translation.x = Input.GetAxis("Horizontal");
        translation.z = Input.GetAxis("Vertical");
        transform.Translate(translation.normalized * Time.deltaTime * 5, Space.Self);

        GetComponent<Rigidbody>().useGravity = true;
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        pitch = Mathf.Clamp(pitch - mouseDelta.y, -88, 88);
        yaw += mouseDelta.x;

        if (Input.GetAxis("Jump") != 0 && !onSky)
        {
            GetComponent<Rigidbody>().velocity += new Vector3(0, 5f, 0);
            onSky = true;
        }

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hitInfo, 10))
        {
            //Debug.Log("Raycasted at" + hitInfo.point.ToString());
            long posZ = transform.rotation.eulerAngles.y < 180 ? Mathf.CeilToInt(hitInfo.point.z) : Mathf.FloorToInt(hitInfo.point.z);

            BlockPos pos = new BlockPos(Mathf.FloorToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y - 1), posZ);
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

    void FixedUpdate()
    {
        Transform cameraTransform = Camera.main.transform;
        var camFactor = Mathf.Clamp01(cameraSmoothing * Time.deltaTime);

        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(pitch, 0, 0), camFactor);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, yaw, 0), camFactor);
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
