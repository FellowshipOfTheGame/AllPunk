using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUICreator : MonoBehaviour
{
    public float pathMultiplier = 1;
    public float lineWidth = 5;
    public float doorSize = 10;
    public float savePrefabSize = 10;

    public GameObject mapPanel;
    public Transform parent;
    public GameObject currentRoomPrefab;
    public GameObject roomPrefab;
    public GameObject pathPrefab;
    public GameObject doorPrefab;
    public GameObject savePrefab;

    public MapInfo mapInfo;

    private Dictionary<string,GameObject> roomDict = new Dictionary<string, GameObject>();
    private Dictionary<UniqueTransition, bool> transitionsDone = new Dictionary<UniqueTransition, bool>();

    public bool PanelActive
    {
        get
        {
            return panelOpen;
        }
        set
        {
            SetPanelActive(value);
        }
    }
    private bool panelOpen = false;
    private bool createdMap = false;

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
        mapPanel.SetActive(false);
    }

    void Update()
    {
        if(createdMap && Input.GetButtonDown("Map"))
        {
            SetPanelActive(!panelOpen);
        }
    }

    public void SetPanelActive(bool active)
    {
        if(panelOpen != active)
        {
            if(active)
                OpenPanel();
            else
                ClosePanel();
        }
    }

    private void OpenPanel()
    {
        if(!scr_HUDController.hudController.canPause) return;

        scr_GameManager.instance.setPauseGame(true);
        scr_HUDController.hudController.canPause = false;
        scr_HUDController.hudController.fadeIn(null);
        mapPanel.SetActive(true);
        panelOpen = true;
    }

    private void ClosePanel()
    {
        scr_GameManager.instance.setPauseGame(false);
        scr_HUDController.hudController.canPause = true;
        scr_HUDController.hudController.fadeOut(null);
        mapPanel.SetActive(false);
        panelOpen = false;
    }

    public string playerRoom;

    public void Generate(StringBoleanDictionary discoveredScenes, string playerRoom = "")
    {
        Debug.Log("Generating room for pr " + playerRoom);
        this.playerRoom = playerRoom;
        //Clean previous generation
        for (int i = parent.childCount-1; i > -1; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        roomDict.Clear();
        transitionsDone.Clear();
        RectTransform rectParent = parent as RectTransform;
        if(rectParent)
        {
            rectParent.anchorMax = Vector2.one;
            rectParent.anchorMin = Vector2.zero;
            rectParent.offsetMax = Vector2.zero;
            rectParent.offsetMin = Vector2.zero;
        }

        Queue<RoomGenOrder> roomQueue = new Queue<RoomGenOrder>();
        //Get first
        Room first = null;
        foreach(var pair in mapInfo.rooms) 
        { 
            if( discoveredScenes.ContainsKey(pair.Value.scene) )
            {
                first = pair.Value; 
                break; 
            }
        }
        if(first == null) return;

        roomQueue.Enqueue(new RoomGenOrder(first,Vector3.zero));

        while(roomQueue.Count > 0)
        {
            RoomGenOrder currentRoom = roomQueue.Dequeue();

            if(!roomDict.ContainsKey(currentRoom.room.scene) && discoveredScenes.ContainsKey(currentRoom.room.scene))
            {
                // Debug.Log("Created " + currentRoom.room.scene);
                //Creates object in the correct position
                GameObject roomObject;
                currentRoom.position.z = currentRoomPrefab.transform.localPosition.z;
                if(playerRoom == currentRoom.room.scene)
                    roomObject = GameObject.Instantiate(currentRoomPrefab, currentRoom.position, Quaternion.identity, parent);
                else
                    roomObject = GameObject.Instantiate(roomPrefab, currentRoom.position, Quaternion.identity, parent);
                roomObject.SetActive(true);
                roomObject.name = currentRoom.room.scene;
                roomObject.transform.localScale = currentRoom.room.bounds.size;
                roomObject.transform.SetAsFirstSibling();
                Image renderer = roomObject.GetComponent<Image>();
                if(renderer) renderer.color = currentRoom.room.color;

                //Create save
                for (int i = 0; i < currentRoom.room.savePoints.Count; i++)
                {
                    GameObject save = GameObject.Instantiate(savePrefab);
                    save.SetActive(true);
                    save.transform.position = currentRoom.position - currentRoom.room.bounds.extents 
                    + Vector3.Scale(V2ToV3(currentRoom.room.savePoints[i].positionPercent), currentRoom.room.bounds.size);

                    save.transform.localScale = save.transform.localScale * savePrefabSize;
                    save.transform.SetParent(parent);
                }

                List<UniqueTransition> transitions = mapInfo.GetTransitionsFromScene(currentRoom.room.scene);

                for (int i = 0; i < transitions.Count; i++)
                {
                    //Avoid doing double transition
                    if(transitionsDone.ContainsKey(transitions[i])) continue;

                    bool isScene1 = (transitions[i].scene1 == currentRoom.room.scene);
                    string otherScene = (isScene1) ? transitions[i].scene2 : transitions[i].scene1;
                    Room otherRoom = mapInfo.rooms[otherScene];

                    //Get start and end position
                    Vector3 startPosition, endPosition;
                    Vector2 thisPercentPos = (isScene1) ? transitions[i].percentPositionScene1 : transitions[i].percentPositionScene2;

                    startPosition = currentRoom.position - currentRoom.room.bounds.extents + Vector3.Scale(V2ToV3(thisPercentPos), currentRoom.room.bounds.size);

                    // If should skip this transition, at least make the door
                    if(QueueContainsScene(roomQueue,otherScene) || !discoveredScenes.ContainsKey(otherScene))
                    {
                        CreateDoor(startPosition,parent);
                        continue;
                    }

                    Vector2 otherPercentPos = (isScene1) ? transitions[i].percentPositionScene2 : transitions[i].percentPositionScene1;
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

                    CreatePath(transitions[i], isScene1, startPosition, endPosition, parent);
                    CreateDoor(endPosition,parent);
                    transitionsDone.Add(transitions[i],true);
                }

                //Add itself to dictionary to avoid duplicate
                roomDict.Add(currentRoom.room.scene, roomObject);
            }
        }

        FixUIScale();

        createdMap = true;
    }

    void FixUIScale()
    {
        Bounds bounds = GetBounds();
        RectTransform parentTransform = parent as RectTransform;
        float parentAspectRatio = parentTransform.rect.height / parentTransform.rect.width;
        float boundsAspectRatio = bounds.size.y / bounds.size.x;
        float correctedRation = boundsAspectRatio / parentAspectRatio;


        parentTransform.anchorMax = new Vector2(1, Mathf.Clamp01(0.5f + correctedRation*0.5f));
        parentTransform.anchorMin = new Vector2(0, Mathf.Clamp01(0.5f - correctedRation*0.5f));
        parentTransform.offsetMax = Vector2.zero;
        parentTransform.offsetMin = Vector2.zero;

        // DebugBounds(bounds);
        for (int i = 0; i < parent.childCount; i++)
        {
            RectTransform child = parent.GetChild(i) as RectTransform;
            if(child == null) continue;

            Vector3 childCenter = child.position;
            Vector3 childExtent = AbsVector3(child.localScale) * 0.5f;

            Vector2 childRelativePos = positionToPorcentage(bounds, childCenter);
            Vector2 childRelativeExtent = Divide(childExtent, bounds.size);
            // childRelativePos.y *= aspectRatio;
            // childRelativeExtent.y *= aspectRatio;

            child.anchorMin = childRelativePos - childRelativeExtent;
            child.anchorMax = childRelativePos + childRelativeExtent;
            child.offsetMax = Vector2.zero;
            child.offsetMin = Vector2.zero;
            child.localScale = Vector3.one;
        }
    }

    void DebugBounds(Bounds bounds)
    {
        Debug.Log("Bounds are: " + bounds);
        Vector2 topRight = positionToPorcentage(bounds,bounds.max);
        Vector2 bottomLeft = positionToPorcentage(bounds,bounds.min);
        RectTransform tr = GameObject.Instantiate(roomPrefab).transform as RectTransform;
        tr.gameObject.SetActive(true);
        tr.SetParent(parent);
        tr.name = "Top Right";

        tr.anchorMax = topRight + new Vector2(0.1f,0.1f);
        tr.anchorMin = topRight - new Vector2(0.1f,0.1f);
        tr.offsetMax = Vector2.zero;
        tr.offsetMin = Vector2.zero;
        tr.ForceUpdateRectTransforms();
        
        
        RectTransform bl = GameObject.Instantiate(roomPrefab).transform as RectTransform;
        bl.gameObject.SetActive(true);
        bl.name = "Bottom Left";
        bl.SetParent(parent);
        bl.anchorMax = bottomLeft + new Vector2(0.1f,0.1f);
        bl.anchorMin = bottomLeft - new Vector2(0.1f,0.1f);
        bl.offsetMax = Vector2.zero;
        bl.offsetMin = Vector2.zero;
        bl.ForceUpdateRectTransforms();

    }

    private Vector3 Divide(Vector3 a, Vector3 b)
    {
        Vector3 dividend = new Vector3(1f/b.x,1f/b.y,1f/b.z);
        return Vector3.Scale(a,dividend);
    }

    private Vector3 AbsVector3(Vector3 a)
    {
        Vector3 result = new Vector3();
        result.x = Mathf.Abs(a.x);
        result.y = Mathf.Abs(a.y);
        result.z = Mathf.Abs(a.z);

        return result;
    }

    private Vector2 positionToPorcentage(Bounds bounds, Vector3 position)
    {
        Vector3 dif = position - bounds.min;
        return new Vector2(dif.x / bounds.size.x, dif.y / bounds.size.y);
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
        startPosition.z = pathPrefab.transform.localPosition.z;
        endPosition.z = pathPrefab.transform.localPosition.z;

        Vector3 currentStart;
        bool isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
        Vector3 divider = (isHorizontal) ? new Vector3(1.0f/2.0f,1.0f,1f) : new Vector3(1.0f,1.0f/2.0f,1f);
        delta = Vector3.Scale(delta,divider);
        currentStart = startPosition;

        for (int i = 0; i < numSteps; i++)
        {
            GameObject path = GameObject.Instantiate(pathPrefab);
            path.SetActive(true);
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
    }

    private void CreateDoor(Vector3 position, Transform parent)
    {
        position.z = doorPrefab.transform.localPosition.z;
        GameObject door = GameObject.Instantiate(doorPrefab, position, Quaternion.identity);
        door.SetActive(true);
        door.transform.localScale = door.transform.localScale * doorSize;
        door.transform.SetParent(parent);
    }

    Vector3 V2ToV3(Vector2 v2)
    {
        return new Vector3(v2.x,v2.y,0);
    }

    Bounds GetBounds()
    {
        Vector3 min = Vector3.zero; 
        Vector3 max = Vector3.zero;
        Vector3 lowerCorner = new Vector3(-0.5f,-0.5f,0);
        Vector3 upperCorner = new Vector3(0.5f,0.5f,0);

        Queue<Transform> mapParts = new Queue<Transform>();

        mapParts.Enqueue(parent);

        while(mapParts.Count > 0)
        {
            Transform current = mapParts.Dequeue();

            Vector3 currentLower = current.TransformPoint(lowerCorner);
            Vector3 currentUpper = current.TransformPoint(upperCorner);
            min.x = Mathf.Min(min.x,currentLower.x);
            min.y = Mathf.Min(min.y,currentLower.y);
            max.x = Mathf.Max(max.x,currentUpper.x);
            max.y = Mathf.Max(max.y,currentUpper.y);

            // Debug.Log("On " + current.name + " Current low was " + currentLower);
            for (int i = 0; i < current.childCount; i++)
            {
                mapParts.Enqueue(current.GetChild(i));
            }

        }

        Vector3 center = (max + min) * 0.5f;
        Vector3 size = (max - min);

        return new Bounds(center, size);
    }
}
