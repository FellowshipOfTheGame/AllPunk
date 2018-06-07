using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(StateDictionary))]
public class StateDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
