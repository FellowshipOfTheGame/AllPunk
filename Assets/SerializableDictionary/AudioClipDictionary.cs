using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[Serializable]
public class AudioClipDictionary : SerializableDictionary<string, scr_AudioClipWrapper> {}