using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUICreator : MonoBehaviour
{
    public Transform parent;
    public GameObject roomPrefab;
    public GameObject pathPrefab;

    public MapInfo mapInfo;

    private Dictionary<string,GameObject> roomDict = new Dictionary<string, GameObject>();

    private class RoomGenOrder
    {
        public Room room;
        public Vector3 position;

        public RoomGenOrder(Room room, Vector3 position)
        {
            this.room = room;
            this.position = position;
        }
    }

    void Start()
    {
        Generate();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }

    public void Generate()
    {
        //Clean previous generation
        for (int i = parent.childCount-1; i > -1; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        roomDict.Clear();

        Stack<RoomGenOrder> stackRoom = new Stack<RoomGenOrder>();
        //Get first
        Room first = null;
        foreach(var pair in mapInfo.rooms) 
        { first = pair.Value; break; }

        stackRoom.Push(new RoomGenOrder(first,Vector3.zero));

        while(stackRoom.Count > 0)
        {
            RoomGenOrder currentRoom = stackRoom.Pop();

            if(!roomDict.ContainsKey(currentRoom.room.scene))
            {
                //Creates object in the correct position
                GameObject roomObject = GameObject.Instantiate(roomPrefab, currentRoom.position, Quaternion.identity, parent);
                roomObject.name = currentRoom.room.scene;
                roomObject.transform.localScale = currentRoom.room.bounds.extents;
                SpriteRenderer renderer = roomObject.GetComponent<SpriteRenderer>();
                if(renderer) renderer.color = currentRoom.room.color;

                //Instantiate all paths and add to the list the next room
                foreach (var transition in currentRoom.room.exits)
                {
                    //Create path
                    Vector3 initialPosition = cornerOfRoom(currentRoom.position,currentRoom.room.bounds) + Vector3.Scale(V2ToV3(transition.Value.positionPercent), currentRoom.room.bounds.size);
                    Vector3 exitPosition = CreatePath(transition.Value, initialPosition, roomObject.transform);

                    GameObject door = GameObject.Instantiate(roomPrefab, initialPosition, Quaternion.identity);
                    door.name = "Door " + transition.Value.targetScene;
                    door.transform.SetParent(roomObject.transform);

                    GameObject doorEnd = GameObject.Instantiate(roomPrefab, exitPosition, Quaternion.identity);
                    doorEnd.name = "DoorEnd " + transition.Value.targetScene;
                    doorEnd.transform.SetParent(roomObject.transform);

                    //Create order for next room
                    Room roomToTransition = mapInfo.rooms[transition.Value.targetScene];
                    Transition entryTransition = roomToTransition.entries[currentRoom.room.scene];
                    Vector3 roomCenter = exitPosition + roomToTransition.bounds.extents - Vector3.Scale(V2ToV3(entryTransition.positionPercent), roomToTransition.bounds.size);
                    stackRoom.Push(new RoomGenOrder(roomToTransition, roomCenter));
                }

                //Add itself to dictionary to avoid duplicate
                roomDict.Add(currentRoom.room.scene, roomObject);
            }
        }

    }

    Vector3 cornerOfRoom(Vector3 position, Bounds bounds)
    {
        return position - bounds.extents;
    }

    /// <summary>
    /// Returns the end of the path position
    /// </summary>
    /// <returns></returns>
    Vector3 CreatePath(Transition transition, Vector3 startPosition, Transform roomTransform)
    {
        Vector3 endPosition = startPosition + new Vector3(transition.offset.x,transition.offset.y,0);
        GameObject pathX = null, pathY = null;

        if(transition.offset.x != 0)
            pathX = GameObject.Instantiate(pathPrefab);
        if(transition.offset.y != 0)
            pathY = GameObject.Instantiate(pathPrefab);

        Vector3 scale;
        if(pathX)
        {
            if(transition.offset.x > transition.offset.y)
                pathX.transform.position = startPosition + Vector3.right * transition.offset.x * 0.5f;
            else
                pathX.transform.position = startPosition + Vector3.up * transition.offset.y + Vector3.right * transition.offset.x * 0.5f;
            scale = pathX.transform.localScale;
            scale.x *= transition.offset.x * 0.5f;
            pathX.transform.localScale = scale;
            pathX.transform.SetParent(roomTransform);
        }
        if(pathY)
        {
            if(transition.offset.x > transition.offset.y)
                pathY.transform.position = startPosition + Vector3.right * transition.offset.x + Vector3.up * transition.offset.y * 0.5f;
            else
                pathY.transform.position = startPosition + Vector3.up * transition.offset.y * 0.5f;
            scale = pathY.transform.localScale;
            scale.y *= transition.offset.y * 0.5f;
            pathY.transform.localScale = scale;
            pathY.transform.SetParent(roomTransform);
        }


        return endPosition;
    }

    Vector3 V2ToV3(Vector2 v2)
    {
        return new Vector3(v2.x,v2.y,0);
    }
}
