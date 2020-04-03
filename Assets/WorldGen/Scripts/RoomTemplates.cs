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

    public GameObject entryRoom;
    public GameObject closedRoom;
    public GameObject endTrigger;

    public List<GameObject> rooms;

    public int DungeonSize;
    public bool exitSpawned = false;

    private void Start()
    {
        Instantiate(entryRoom, transform.position, Quaternion.identity);
    }
}
