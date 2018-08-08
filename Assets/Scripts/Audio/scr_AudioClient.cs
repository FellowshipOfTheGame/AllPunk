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
		music,	//AudioManager's Music Audiosource
		voice	//AudioManager's Voice Audiosource
	};

	/// <summary>
	/// The key list. Used to get a random element of the dictionary
	/// </summary>
	private	List<string> keyList;
	#endregion


	[SerializeField]
	[Tooltip("Optional field")]
	AudioSource localAudiosource;

	[SerializeField]
	AudioClipDictionary audioClips;

	void Awake(){
		keyList = new List <string> (audioClips.Keys);
	}

	/// <summary>
	/// Checks to see if there is a local audio source.
	/// </summary>
	/// <returns><c>true</c>, if local audio source exists, <c>false</c> otherwise.</returns>
	public bool hasLocalAudioSource(){
		if (localAudiosource != null)
			return true;
		else
			return false;
	}

	public void playLocalClip(string key){
		playAudioClip(key, sources.local);
	}

	/// <summary>
	/// Plays an audio clip sotred in the clips dictionary
	/// </summary>
	/// <returns><c>true</c>, if audio clip was played, <c>false</c> otherwise.</returns>
	/// <param name="key">Key.</param>
	/// <param name="source">Audiosource to play in.</param>
	public bool playAudioClip(string key, scr_AudioClient.sources source){
		if (!audioClips.ContainsKey (key)) {
			Debug.LogWarning ("AudioClient Warning - Key not found!");
			return false;
		}

		scr_AudioClipWrapper wrapper;
		audioClips.TryGetValue (key, out wrapper);

		switch (source) {
		case sources.local:
			localAudiosource.volume = wrapper.volume;
			localAudiosource.pitch = wrapper.pitch;
			localAudiosource.PlayOneShot (wrapper.clip, wrapper.volume);
			return true;
		default:
			return scr_AudioManager.instance.playClipOnce (wrapper, source);
		}

	}

	public bool playLoopClip(string key, scr_AudioClient.sources source){
		if (!audioClips.ContainsKey (key)) {
			Debug.LogWarning ("AudioClient Warning - Key not found!");
			return false;
		}

		scr_AudioClipWrapper wrapper;
		audioClips.TryGetValue (key, out wrapper);

		switch (source) {
		case sources.local:
			localAudiosource.clip = wrapper.clip;
			localAudiosource.loop = wrapper.loop;
			localAudiosource.pitch = wrapper.pitch;
			localAudiosource.Play();
			return true;
		default:
			return scr_AudioManager.instance.playClipOnce (wrapper, source);
		}

	}

	public void stoplocalClip(){
		if(localAudiosource != null){
			localAudiosource.Stop();
		}
	}

	/// <summary>
	/// Plays a random clip stored in the dictionary
	/// </summary>
	/// <returns><c>true</c>, if random clip was played, <c>false</c> otherwise.</returns>
	/// <param name="source">Source.</param>
	public bool playRandomClip(scr_AudioClient.sources source){
		scr_AudioClipWrapper wrapper;
		audioClips.TryGetValue ( keyList[Random.Range(0,keyList.Count-1)], out wrapper );

		switch (source) {
		case sources.local:
			if (localAudiosource != null) {
				localAudiosource.PlayOneShot (wrapper.clip, wrapper.volume);
				return true;
			} else {
				Debug.LogError ("Audio Client Error - No local AudioSource!");
				return false;
			}
		default:
			return scr_AudioManager.instance.playClipOnce (wrapper, source);
		}
	}

	public scr_AudioClipWrapper getWrapper(string key){
		scr_AudioClipWrapper wrapper;
		audioClips.TryGetValue (key, out wrapper);
		return wrapper;
	}
		
}
