using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using FSM;

[Serializable]
public class StateDictionary : SerializableDictionary<string, State> {}
