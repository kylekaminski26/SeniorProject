using UnityEngine;
using Pathfinding;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1--> need bottom door
    // 2--> need top door
    // 3--> need left door
    // 4--> need right door

    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;
    public float waitTime = 5f;

    private void Start()
    {
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.4f);
    }

    private void Spawn()
    {
        if (spawned == false)
        {
            if (templates.DungeonSize > templates.rooms.Count)
            {
                SpawnRooms();
            }
            else
            {
                SpawnEndRooms();
                SpawnExit();

                AstarPath.active.Scan();
            }
        }
        spawned = true;
    }

    private void SpawnRooms()
    {
        if (openingDirection == 1)
        {
            // Need to spawn a room with a BOTTOM door
            rand = Random.Range(0, templates.bottomRooms.Length);
            Instantiate(templates.bottomRooms[rand], transform.position, Quaternion.identity);
        }
        else if (openingDirection == 2)
        {
            // Need to spawn a room with a TOP door
            rand = Random.Range(0, templates.topRooms.Length);
            Instantiate(templates.topRooms[rand], transform.position, Quaternion.identity);
        }
        else if (openingDirection == 3)
        {
            // Need to spawn a room with a LEFT door
            rand = Random.Range(0, templates.leftRooms.Length);
            Instantiate(templates.leftRooms[rand], transform.position, Quaternion.identity);
        }
        else if (openingDirection == 4)
        {
            // Need to spawn a room with a RIGHT door
            rand = Random.Range(0, templates.rightRooms.Length);
            Instantiate(templates.rightRooms[rand], transform.position, Quaternion.identity);
        }
    }

    private void SpawnEndRooms()
    {
        if (openingDirection == 1)
        {
            // Need to spawn a room with a BOTTOM door
            rand = Random.Range(0, templates.bottomRoomsEnd.Length);
            Instantiate(templates.bottomRoomsEnd[rand], transform.position, Quaternion.identity);
        }
        else if (openingDirection == 2)
        {
            // Need to spawn a room with a TOP door
            rand = Random.Range(0, templates.topRoomsEnd.Length);
            Instantiate(templates.topRoomsEnd[rand], transform.position, Quaternion.identity);
        }
        else if (openingDirection == 3)
        {
            // Need to spawn a room with a LEFT door
            rand = Random.Range(0, templates.leftRoomsEnd.Length);
            Instantiate(templates.leftRoomsEnd[rand], transform.position, Quaternion.identity);
        }
        else if (openingDirection == 4)
        {
            // Need to spawn a room with a RIGHT door
            rand = Random.Range(0, templates.rightRoomsEnd.Length);
            Instantiate(templates.rightRoomsEnd[rand], transform.position, Quaternion.identity);
        }
    }

    private void SpawnExit()
    {
        //spawns the End Trigger only if this is the FIRST Terminating Room
        if (!templates.exitSpawned)
        {
            Instantiate(templates.endTrigger, transform.position, Quaternion.identity);
            templates.exitSpawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
         if (collision.CompareTag("SpawnPoint") && collision.GetComponent<RoomSpawner>().spawned == true)
         {
             Destroy(gameObject);
         }*/
         
        if (collision.CompareTag("SpawnPoint"))
        {
            if (collision.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Instantiate(templates.entryRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            spawned = true;
        }
    }
}
