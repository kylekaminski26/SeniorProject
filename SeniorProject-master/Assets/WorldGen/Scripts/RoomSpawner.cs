using UnityEngine;

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

    private GameObject room;

    private void Start()
    {
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
        transform.parent = templates.roomListContainer.transform;
        Invoke("Spawn", 0.2f);
    }

    private void Spawn()
    {
        if (spawned == false)
        {
            if (templates.dungeonSize > templates.rooms.Count)
            {
                SpawnRooms();
                SpawnOverlays();
            }
            else
            {
                SpawnEndRooms();
                CalculateMinMaxXY();

                //spawn exit in the first End Room
                if (!templates.exitSpawned)
                {
                    SpawnExit();
                }
                else
                {
                    SpawnOverlays();
                }
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
            room = Instantiate(templates.bottomRooms[rand], transform.position, Quaternion.identity);
            setParent();
        }
        else if (openingDirection == 2)
        {
            // Need to spawn a room with a TOP door
            rand = Random.Range(0, templates.topRooms.Length);
            room = Instantiate(templates.topRooms[rand], transform.position, Quaternion.identity);
            setParent();
        }
        else if (openingDirection == 3)
        {
            // Need to spawn a room with a LEFT door
            rand = Random.Range(0, templates.leftRooms.Length);
            room = Instantiate(templates.leftRooms[rand], transform.position, Quaternion.identity);
            setParent();
        }
        else if (openingDirection == 4)
        {
            // Need to spawn a room with a RIGHT door
            rand = Random.Range(0, templates.rightRooms.Length);
            room = Instantiate(templates.rightRooms[rand], transform.position, Quaternion.identity);
            setParent();
        }
    }

    private void SpawnEndRooms()
    {
        if (openingDirection == 1)
        {
            // Need to spawn a room with a BOTTOM door
            rand = Random.Range(0, templates.bottomRoomsEnd.Length);
            room = Instantiate(templates.bottomRoomsEnd[rand], transform.position, Quaternion.identity);
            setParent();
        }
        else if (openingDirection == 2)
        {
            // Need to spawn a room with a TOP door
            rand = Random.Range(0, templates.topRoomsEnd.Length);
            room = Instantiate(templates.topRoomsEnd[rand], transform.position, Quaternion.identity);
            setParent();
        }
        else if (openingDirection == 3)
        {
            // Need to spawn a room with a LEFT door
            rand = Random.Range(0, templates.leftRoomsEnd.Length);
            room = Instantiate(templates.leftRoomsEnd[rand], transform.position, Quaternion.identity);
            setParent();
        }
        else if (openingDirection == 4)
        {
            // Need to spawn a room with a RIGHT door
            rand = Random.Range(0, templates.rightRoomsEnd.Length);
            room = Instantiate(templates.rightRoomsEnd[rand], transform.position, Quaternion.identity);
            setParent();
        }
    }

    private void setParent()
    {
        room.transform.parent = templates.roomListContainer.transform;
    }

    public void SpawnOverlays()
    {
        //don't generate overlay in the spawn room
        if(!(transform.position.x == 0 && transform.position.y == 0))
        {
            rand = Random.Range(0, templates.overlays.Length);
            Instantiate(templates.overlays[rand], transform.position, Quaternion.identity);
        }
    }

    public void SpawnExit()
    {
        Instantiate(templates.endOverlay, transform.position, Quaternion.identity);
        templates.exitSpawned = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpawnPoint"))
        {
            if (collision.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                room = Instantiate(templates.entryRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
                setParent();
                SpawnOverlays();
            }
            spawned = true;
        }
    }

    private void CalculateMinMaxXY()
    {
        if (transform.position.x > templates.maxX)
        {
            templates.maxX = (int) transform.position.x;
        }
        else if (transform.position.x < templates.minX)
        {
            templates.minX = (int) transform.position.x;
        }
        if (transform.position.y > templates.maxY)
        {
            templates.maxY = (int) transform.position.y;
        }
        else if (transform.position.y < templates.minY)
        {
            templates.minY = (int) transform.position.y;
        }
    }
}
