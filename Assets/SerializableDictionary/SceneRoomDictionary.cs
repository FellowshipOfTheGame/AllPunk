using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneRoomDictionary : SerializableDictionary<string,Room>
{
}

[Serializable]
public class SceneTransitionDictionary : SerializableDictionary<string,Transition>
{
}