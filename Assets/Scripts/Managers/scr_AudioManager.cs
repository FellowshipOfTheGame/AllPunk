using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class scr_AudioManager : MonoBehaviour {

	#region variables
	public static scr_AudioManager instance = null;
	public AudioSource sfxSource;
	public AudioSource musicSoruce;
	public AudioSource voiceSource;
	#endregion


	/// <summary>
	/// Plays the clip once. Method to be called by the scr_AudioClient
	/// </summary>
	/// <returns><c>true</c>, if clip was played, <c>false</c> otherwise.</returns>
	/// <param name="wrapper">Wrapper.</param>
	/// <param name="source">Source.</param>
	public bool playClipOnce(scr_AudioClipWrapper wrapper, scr_AudioClient.sources source){
		switch (source) {
		case scr_AudioClient.sources.sfx:
			sfxSource.PlayOneShot (wrapper.clip, wrapper.volume);
			return true;
		case scr_AudioClient.sources.music:
			musicSoruce.clip = wrapper.clip;
			musicSoruce.volume = wrapper.volume;
			musicSoruce.pitch = wrapper.pitch;
			musicSoruce.loop = wrapper.loop;
			musicSoruce.Play ();
			return true;
		case scr_AudioClient.sources.voice:
			voiceSource.pitch = wrapper.pitch;
			voiceSource.PlayOneShot (wrapper.clip, wrapper.volume);
			return true;
		default:
			Debug.LogWarning ("AudioManager playClipOnce: source not found");
			return false;
		}
	}

	void Awake () {
		
		if (instance == null)
			instance = this;  		
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		if (sfxSource == null || musicSoruce == null || voiceSource == null)
			Debug.LogError ("AudioManager Error: null AudioSources");

	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.T)) {
			sfxSource.PlayOneShot (sfxSource.clip);
		}
	}
		

}
