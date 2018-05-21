﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Audio clip wrapper.
/// Wraps an audio clip and adds parameters to play on the AudioSource
/// </summary>

[Serializable]
public class scr_AudioClipWrapper {

	public AudioClip clip;

	[Range(0f,1f)]
	public float volume;

	[Range(.1f,3f)]
	public float pitch;

	public bool loop;
}
