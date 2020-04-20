using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject[] bottomRoomsEnd;
    public GameObject[] topRoomsEnd;
    public GameObject[] leftRoomsEnd;
    public GameObject[] rightRoomsEnd;

    public GameObject[] overlays;

    public GameObject entryRoom;
    public GameObject closedRoom;
    public GameObject endOverlay;

    public List<GameObject> rooms;

    public int dungeonSize;
    public float waitTime;
    public bool exitSpawned = false;

    private bool scannedBoard = false;
    public int maxX, minX, maxY, minY = 0;

    private void Start()
    {
        Instantiate(entryRoom, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (!scannedBoard)
        {
            if (waitTime <= 0)
            {
                SetAndTriggerAStarScan();
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    private void SetAndTriggerAStarScan()
    {
        var graph = AstarPath.active.data.gridGraph;

        //get the largest absolute value of max/min
        int width = maxX > Mathf.Abs(minX) ? maxX : Mathf.Abs(minX);
        int depth = maxY > Mathf.Abs(minY) ? maxY : Mathf.Abs(minY);

        float nodeSize = 1;

        //Account for the room spawnpoint to the walls (x + 10), double it for the actual width of the entire board
        width = (width + 10) * 2;
        depth = (depth + 10) * 2;

        graph.SetDimensions(width, depth, nodeSize);

        AstarPath.active.Scan();
        scannedBoard = true;
    }
}
