using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public float playerSize = 2f;
    public float playerCursorPercent = 0.1f;

    public RectTransform parent;
    public RectTransform roomRef;
    public RectTransform cursor;

    public GameObject roomPrefab;
    public GameObject doorPrefab;
    public GameObject savePrefab;
    public GameObject cursorPrefab;

    public MapInfo mapInfo;

    private Transform target;
    private string currentScene;
    private bool mapGenerated = false;
    private Bounds roomBounds;

    private Vector2 playerExtent;
    private Vector2 cursorExtent;

    public void StartMinimap(Transform target, string currentScene)
    {
        parent.gameObject.SetActive(true);
        this.target = target;
        this.currentScene = currentScene;
        mapGenerated = false;

        SetupMap();
    }

    public void StopMinimap()
    {
        parent.gameObject.SetActive(false);
        this.target = null;
    }

    private void SetupMap()
    {
        //Clean previous generation
        for (int i = this.parent.childCount-1; i > -1; i--)
        {
            Transform child = this.parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        parent.anchorMax = Vector2.one;
        parent.anchorMin = Vector2.zero;
        parent.offsetMax = Vector2.zero;
        parent.offsetMin = Vector2.zero;

        roomRef = GameObject.Instantiate(roomPrefab).transform as RectTransform;
        roomRef.gameObject.SetActive(true);
        roomRef.SetParent(parent);
        
        UnityEngine.UI.Image image = roomRef.GetComponent<UnityEngine.UI.Image>();
        if(image) image.color = mapInfo.rooms[currentScene].color;
        
        roomBounds = mapInfo.rooms[currentScene].bounds;

        float parentAspect, roomAspect, realAspect;
        bool isHorizontal = roomBounds.size.x > roomBounds.size.y;
        if(isHorizontal)
        {
            parentAspect = parent.rect.y / parent.rect.x;
            roomAspect = roomBounds.size.y / roomBounds.size.x;
            realAspect = roomAspect / parentAspect;
            parent.anchorMax = new Vector2(1f ,0.5f + realAspect * 0.5f);
            parent.anchorMin = new Vector2(0f ,0.5f - realAspect * 0.5f);
        }
        else
        {
            parentAspect = parent.rect.x / parent.rect.y;
            roomAspect = roomBounds.size.x / roomBounds.size.y;
            realAspect = roomAspect / parentAspect;
            parent.anchorMax = new Vector2(0.5f + realAspect * 0.5f, 1f);
            parent.anchorMin = new Vector2(0.5f - realAspect * 0.5f, 0f);
        }

        roomRef.anchorMax = Vector2.one;
        roomRef.anchorMin = Vector2.zero;
        roomRef.offsetMax = Vector2.zero;
        roomRef.offsetMin = Vector2.zero;

        //Use 2 as size of player
        float sizeScale = playerSize / roomBounds.size.y;
        playerExtent = new Vector2(sizeScale,sizeScale);

        cursor = GameObject.Instantiate(cursorPrefab).transform as RectTransform;
        cursor.SetParent(roomRef);
        cursor.gameObject.SetActive(true);
        cursorExtent = new Vector2(playerCursorPercent,playerCursorPercent);

        foreach (var exit in mapInfo.rooms[currentScene].exits)
        {
            RectTransform newDoor = GameObject.Instantiate(doorPrefab).transform as RectTransform;
            newDoor.gameObject.SetActive(true);
            newDoor.SetParent(roomRef);
            newDoor.anchorMax = exit.Value.positionPercent + playerExtent;
            newDoor.anchorMin = exit.Value.positionPercent - playerExtent;
            newDoor.offsetMax = Vector2.zero;
            newDoor.offsetMin = Vector2.zero;
        }

        foreach (var save in mapInfo.rooms[currentScene].savePoints)
        {
            RectTransform newSavePoint = GameObject.Instantiate(savePrefab).transform as RectTransform;
            newSavePoint.gameObject.SetActive(true);
            newSavePoint.SetParent(roomRef);
            newSavePoint.anchorMax = save.positionPercent + playerExtent;
            newSavePoint.anchorMin = save.positionPercent - playerExtent;
            newSavePoint.offsetMax = Vector2.zero;
            newSavePoint.offsetMin = Vector2.zero;
        }

        mapGenerated = true;
    }

    void Update()
    {
        if(target != null && mapGenerated)
        {
            Vector2 percentPos = positionToPorcentage(roomBounds,target.position);

            cursor.anchorMax = percentPos + cursorExtent;
            cursor.anchorMin = percentPos - cursorExtent;
            cursor.offsetMax = Vector2.zero;
            cursor.offsetMin = Vector2.zero;
        }
    }

        private Vector2 positionToPorcentage(Bounds bounds, Vector3 position)
    {
        Vector3 dif = position - bounds.min;
        return new Vector2(dif.x / bounds.size.x, dif.y / bounds.size.y);
    }
}
