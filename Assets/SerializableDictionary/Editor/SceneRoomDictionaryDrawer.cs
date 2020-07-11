using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneRoomDictionary))]
public class SceneRoomDictionaryDrawer : SerializableDictionaryPropertyDrawer
{
}

[CustomPropertyDrawer(typeof(SceneTransitionDictionary))]
public class SceneTransitionDictionaryDrawer : SerializableDictionaryPropertyDrawer
{
}
