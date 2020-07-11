using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Converts a string property into a Scene property in the inspector
/// </summary>
public class SceneAttribute : PropertyAttribute { }

[CreateAssetMenu(fileName = "Map", menuName = "Allpunk/Map", order = 1)]
[Serializable]
public class MapInfo : ScriptableObject
{
    const float transitionDefaultSize = 200;
    public SceneRoomDictionary rooms = new SceneRoomDictionary();

    [ContextMenu("Get all scenes from build")]
    void GetScenesFromBuild()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;     
        string[] scenes = new string[sceneCount];
        for( int i = 0; i < sceneCount; i++ )
        {
            scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            Debug.Log("Scene " + scenes[i]);
            scenes[i] = RemoveExtension(scenes[i]);
        }

        for (int i = 0; i < scenes.Length; i++)
        {
            Room room = new Room();
            room.scene = scenes[i];
            rooms.Add(scenes[i], room);
        }
    }

    [ContextMenu("Get information from rooms")]
    void FillInformationFromScenes()
    {
        foreach(var pair in rooms)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + pair.Key + ".unity");

            //Get room extension
            //Get all tile maps with collider (floor and wall tiles). Choses biggest dimension
            Tilemap[] tilemaps = GameObject.FindObjectsOfType<Tilemap>();
            float biggestArea = 0;
            int biggestIndex = -1;

            for (int i = 0; i < tilemaps.Length; i++)
            {
                if(tilemaps[i].GetComponent<TilemapCollider2D>() != null)
                {
                    Bounds bounds = tilemaps[i].localBounds;
                    Vector2 dimension = (bounds.size);
                    float area = dimension.x * dimension.y;
                    if(biggestArea < area)
                    {
                        biggestIndex = i;
                        biggestArea = area;
                    }
                }
            }

            if(biggestIndex == -1)
                continue;

            Tilemap bestTilemap = tilemaps[biggestIndex];
            pair.Value.bounds = new Bounds(bestTilemap.localBounds.center, bestTilemap.size);
            pair.Value.color = bestTilemap.color;

            // Find all the exits, given by transition area
            scr_TransitionArea[] transitionAreas = GameObject.FindObjectsOfType<scr_TransitionArea>();
            for (int i = 0; i < transitionAreas.Length; i++)
            {
                Transition transition = new Transition();
                transition.originScene = pair.Value.scene;
                transition.targetScene = transitionAreas[i].destinyScene;
                transition.positionPercent = positionToPorcentage(pair.Value.bounds,
                bestTilemap.WorldToLocal(transitionAreas[i].transform.position));
                transition.offset = getOffsetFromPositionPercent(transition.positionPercent);

                pair.Value.exits.Add(transition.targetScene, transition);
            }

            scr_SceneManager sceneManager = GameObject.FindObjectOfType<scr_SceneManager>();
            if(sceneManager != null)
            {
                for (int i = 0; i < sceneManager.neighboorScenesReceive.Length; i++)
                {
                    Transition transition = new Transition();
                    transition.originScene = sceneManager.neighboorScenesReceive[i];
                    transition.targetScene = pair.Value.scene;
                    transition.positionPercent = positionToPorcentage(pair.Value.bounds, 
                    bestTilemap.WorldToLocal(sceneManager.neighboorScenesDestination[i].position));
                    transition.offset = getOffsetFromPositionPercent(transition.positionPercent);

                    pair.Value.entries.Add(transition.originScene, transition);
                }
            }

            scr_SaveStation[] savePoints = GameObject.FindObjectsOfType<scr_SaveStation>();
            for (int i = 0; i < savePoints.Length; i++)
            {
                SavePointInfo savePoint = new SavePointInfo();
                savePoint.recoverEnergy = savePoints[i].recoverEnergy;
                savePoint.recoverHealth = savePoints[i].recoverHP;

                savePoint.positionPercent = positionToPorcentage(pair.Value.bounds, 
                    bestTilemap.WorldToLocal(savePoints[i].transform.position));
                
                pair.Value.savePoints.Add(savePoint);
            }
        }
    }

    private string RemoveExtension(string path)
    {
        string[] parts = path.Split('.');
        return parts[0].Remove(0,14);
    }

    private Vector2 positionToPorcentage(Bounds bounds, Vector3 position)
    {
        Vector3 dif = position - bounds.min;
        return new Vector2(dif.x / bounds.size.x, dif.y / bounds.size.y);
    }

    //Make a X division in square to know whitch direction is which
    private Vector2 getOffsetFromPositionPercent(Vector2 percent)
    {
        float x = percent.x; float y = percent.y;
        int signal = (x > (1 - y)) ? 1 : -1;
        bool isRight = (x <= 0.5f) ? (x < y || x > (1- y)) : (x < (1-y) || x > y);
        Vector2 dir = (isRight) ? Vector2.right : Vector2.up;

        return dir * signal * transitionDefaultSize;
    }
}

[Serializable]
public class Room
{
    public string scene;

    public Bounds bounds;
    public Color color;
    
    public SceneTransitionDictionary exits = new SceneTransitionDictionary();
    public SceneTransitionDictionary entries = new SceneTransitionDictionary();

    public List<SavePointInfo> savePoints = new List<SavePointInfo>();
}

[Serializable]
public class Transition
{
    public string targetScene;
    public string originScene;
    public Vector2 positionPercent;
    public Vector2 offset;
}

[Serializable]
public class SavePointInfo
{
    public Vector2 positionPercent;
    public bool recoverHealth;
    public bool recoverEnergy;
}
