using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AudioClipDictionary))]
public class AudioClipDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
