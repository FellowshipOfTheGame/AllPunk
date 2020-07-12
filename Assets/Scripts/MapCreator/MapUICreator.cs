using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUICreator : MonoBehaviour
{
    public float pathMultiplier = 1;
    public float lineWidth = 5;
    public float doorSize = 10;
    public Transform parent;
    public GameObject roomPrefab;
    public GameObject pathPrefab;
    public GameObject doorPrefab;



    public MapInfo mapInfo;

    private Dictionary<string,GameObject> roomDict = new Dictionary<string, GameObject>();
    private Dictionary<UniqueTransition, bool> transitionsDone = new Dictionary<UniqueTransition, bool>();

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
        Debug.Log("Generated");
        //Clean previous generation
        for (int i = parent.childCount-1; i > -1; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        roomDict.Clear();
        transitionsDone.Clear();

        Queue<RoomGenOrder> roomQueue = new Queue<RoomGenOrder>();
        //Get first
        Room first = null;
        foreach(var pair in mapInfo.rooms) 
        { first = pair.Value; break; }

        roomQueue.Enqueue(new RoomGenOrder(first,Vector3.zero));

        while(roomQueue.Count > 0)
        {
            RoomGenOrder currentRoom = roomQueue.Dequeue();

            if(!roomDict.ContainsKey(currentRoom.room.scene))
            {
                Debug.Log("Created " + currentRoom.room.scene);
                //Creates object in the correct position
                GameObject roomObject = GameObject.Instantiate(roomPrefab, currentRoom.position, Quaternion.identity, parent);
                roomObject.name = currentRoom.room.scene;
                roomObject.transform.localScale = currentRoom.room.bounds.size;
                SpriteRenderer renderer = roomObject.GetComponent<SpriteRenderer>();
                if(renderer) renderer.color = currentRoom.room.color;

                List<UniqueTransition> transitions = mapInfo.GetTransitionsFromScene(currentRoom.room.scene);

                for (int i = 0; i < transitions.Count; i++)
                {
                    //Avoid doing double transition
                    if(transitionsDone.ContainsKey(transitions[i])) continue;

                    bool isScene1 = (transitions[i].scene1 == currentRoom.room.scene);
                    string otherScene = (isScene1) ? transitions[i].scene2 : transitions[i].scene1;
                    Room otherRoom = mapInfo.rooms[otherScene];

                    if(QueueContainsScene(roomQueue,otherScene)) continue;


                    Vector2 thisPercentPos = (isScene1) ? transitions[i].percentPositionScene1 : transitions[i].percentPositionScene2;
                    Vector2 otherPercentPos = (isScene1) ? transitions[i].percentPositionScene2 : transitions[i].percentPositionScene1;

                    //Get start and end position
                    Vector3 startPosition, endPosition;

                    startPosition = currentRoom.position - currentRoom.room.bounds.extents + Vector3.Scale(V2ToV3(thisPercentPos), currentRoom.room.bounds.size);
                    
                    GameObject otherRoomGO = (roomDict.ContainsKey(otherScene)) ? roomDict[otherScene] : null;
                    if(otherRoomGO != null)
                    {
                        endPosition = otherRoomGO.transform.position - otherRoom.bounds.extents + Vector3.Scale(V2ToV3(otherPercentPos), otherRoom.bounds.size);
                    }
                    else
                    {
                        endPosition = startPosition + V2ToV3(transitions[i].defaultOffset) * pathMultiplier;
                        Vector3 roomCenter = endPosition + otherRoom.bounds.extents - Vector3.Scale(V2ToV3(otherPercentPos), otherRoom.bounds.size);
                        roomQueue.Enqueue(new RoomGenOrder(otherRoom, roomCenter));
                    }

                    CreatePath(transitions[i], isScene1, startPosition, endPosition, roomObject.transform);
                    transitionsDone.Add(transitions[i],true);
                }

                //Add itself to dictionary to avoid duplicate
                roomDict.Add(currentRoom.room.scene, roomObject);

                // //Instantiate all paths and add to the list the next room
                // foreach (var transition in currentRoom.room.exits)
                // {
                //     //Create path
                //     Vector3 initialPosition = currentRoom.position - currentRoom.room.bounds.extents + Vector3.Scale(V2ToV3(transition.Value.positionPercent), currentRoom.room.bounds.size);
                //     Vector3 exitPosition = CreatePath(transition.Value, initialPosition, roomObject.transform);

                //     GameObject door = GameObject.Instantiate(roomPrefab, initialPosition, Quaternion.identity);
                //     door.name = "Door " + transition.Value.targetScene;
                //     door.transform.SetParent(roomObject.transform);

                //     GameObject doorEnd = GameObject.Instantiate(roomPrefab, exitPosition, Quaternion.identity);
                //     doorEnd.name = "DoorEnd " + transition.Value.targetScene;
                //     doorEnd.transform.SetParent(roomObject.transform);

                //     //Create order for next room
                //     Room roomToTransition = mapInfo.rooms[transition.Value.targetScene];
                //     Transition entryTransition = roomToTransition.entries[currentRoom.room.scene];
                //     Vector3 roomCenter = exitPosition + roomToTransition.bounds.extents - Vector3.Scale(V2ToV3(entryTransition.positionPercent), roomToTransition.bounds.size);
                //     stackRoom.Push(new RoomGenOrder(roomToTransition, roomCenter));
                // }

            }
        }

    }

    bool QueueContainsScene(Queue<RoomGenOrder> queue, string scene)
    {
        foreach (var item in queue)
        {
            if(item.room.scene == scene)
                return true;
        }
        return false;
    }

    Vector3 cornerOfRoom(Vector3 position, Bounds bounds)
    {
        return position - bounds.extents;
    }

    /// <summary>
    /// Returns the end of the path position
    /// </summary>
    /// <returns></returns>
    void CreatePath(UniqueTransition transition, bool isScene1, Vector3 startPosition, Vector3 endPosition, Transform roomTransform)
    {
        int numSteps = 3;
        Vector3 delta = endPosition - startPosition;
        
        Vector3 currentStart;
        bool isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
        Vector3 divider = (isHorizontal) ? new Vector3(1.0f/2.0f,1.0f,1f) : new Vector3(1.0f,1.0f/2.0f,1f);
        delta = Vector3.Scale(delta,divider);
        currentStart = startPosition;

        for (int i = 0; i < numSteps; i++)
        {
            GameObject path = GameObject.Instantiate(pathPrefab);
            if(isHorizontal)
            {
                path.transform.position = currentStart + Vector3.right * delta.x * 0.5f;
                currentStart += Vector3.right * delta.x;
                Vector3 scale = path.transform.localScale;
                scale.x *= delta.x;
                scale.y *= lineWidth;
                path.transform.localScale = scale;
            }
            else
            {
                path.transform.position = currentStart + Vector3.up * delta.y * 0.5f;
                currentStart += Vector3.up * delta.y;
                Vector3 scale = path.transform.localScale;
                scale.y *= delta.y;
                scale.x *= lineWidth;
                path.transform.localScale = scale;
            }
            path.transform.SetParent(roomTransform);
            isHorizontal = !isHorizontal;
        }

        if( (!isScene1 && transition.canGoFrom2To1) || (isScene1 && transition.canGoFrom1To2) )
        {
            GameObject door = GameObject.Instantiate(doorPrefab, startPosition, Quaternion.identity);
            door.transform.localScale = door.transform.localScale * doorSize;
            door.transform.SetParent(roomTransform);
        }
        if( (isScene1 && transition.canGoFrom2To1) || (!isScene1 && transition.canGoFrom1To2) )
        {
            GameObject door = GameObject.Instantiate(doorPrefab, endPosition, Quaternion.identity);
            door.transform.localScale = door.transform.localScale * doorSize;
            door.transform.SetParent(roomTransform);
        }
    }

    Vector3 V2ToV3(Vector2 v2)
    {
        return new Vector3(v2.x,v2.y,0);
    }
}
