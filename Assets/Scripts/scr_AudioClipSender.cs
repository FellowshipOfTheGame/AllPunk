using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scr audio clip sender.
/// Invokes the play methods
/// </summary>
public class scr_AudioClipSender : MonoBehaviour {

	[SerializeField]
	AudioClipDictionary audioClips;

	/// <summary>
	/// Plays the audio clip in the global Manager
	/// </summary>
	/// <param name="key">Key of the audio</param>
	public bool playAudioClip(string key, scr_AudioManager.sources source){
		if (!audioClips.ContainsKey (key))
			return false;


		//scr_AudioManager.instance.sfxSource;
		return true;
	}

	
	
}
