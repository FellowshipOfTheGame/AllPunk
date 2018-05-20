using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class scr_AudioManager : MonoBehaviour {

	#region variables
	public static scr_AudioManager instance = null;
	public AudioSource sfxSource;
	public AudioSource musicSoruce;

	public enum sources{
		sfx,
		music
	};
	#endregion

	public bool playAudio(){
		return true;
	}

	void Awake () {
		
		if (instance == null)
			instance = this;  		
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		if (sfxSource == null || musicSoruce == null)
			Debug.LogError ("AudioManager Error: null AudioSources");

	}

}
