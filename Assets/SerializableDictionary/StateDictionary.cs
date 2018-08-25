using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

[Serializable]
public class StateDictionary : SerializableDictionary<string, FSM.State> {}
