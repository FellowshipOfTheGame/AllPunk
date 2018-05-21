using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio client. Stores all audioclips that a gameobject needs in a serializable dictionary.
/// Middleman between the AudioManager / local AudioSource and any other script in that object
/// </summary>
public class scr_AudioClient : MonoBehaviour {

	#region variables
	/// <summary>
	/// Audiosources to play clips in.
	/// </summary>
	public enum sources{
		local,	//The Local audiosource
		sfx,	//AudioManager's SFX Audiosource
		music	//AudioManager's Music Audiosource
	};
	#endregion


	[SerializeField]
	[Tooltip("Optional field")]
	AudioSource localAudiosource;

	[SerializeField]
	AudioClipDictionary audioClips;

	void Awake(){
		if (localAudiosource == null) {
			localAudiosource.GetComponent<AudioSource> ();
		}
	}


	/// <summary>
	/// Plays an audio clip sotred in the clips dictionary
	/// </summary>
	/// <returns><c>true</c>, if audio clip was played, <c>false</c> otherwise.</returns>
	/// <param name="key">Key.</param>
	/// <param name="source">Audiosource to play in.</param>
	bool playAudioClip(string key, scr_AudioClient.sources source){
		if (!audioClips.ContainsKey (key))
			return false;

		scr_AudioClipWrapper wrapper;
		audioClips.TryGetValue (key, out wrapper);

		switch (source) {
		case sources.local:
			localAudiosource.PlayOneShot (wrapper.clip, wrapper.volume);
			return true;
		default:
			return scr_AudioManager.instance.playClipOnce (wrapper, source);
		}

	}
		
}
