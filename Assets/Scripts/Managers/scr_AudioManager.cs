using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class scr_AudioManager : MonoBehaviour {

	#region variables
	public static scr_AudioManager instance = null;
	public AudioSource sfxSource;
	public AudioSource musicSource;
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
			musicSource.clip = wrapper.clip;
			musicSource.volume = wrapper.volume;
			musicSource.pitch = wrapper.pitch;
			musicSource.loop = wrapper.loop;
			musicSource.Play();
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

		if (sfxSource == null || musicSource == null || voiceSource == null)
			Debug.LogError ("AudioManager Error: null AudioSources");

	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.T)) {
			sfxSource.PlayOneShot (sfxSource.clip);
		}
	}

	public bool isPlayingMusic(){
		return musicSource.isPlaying;
	}

	public void changeToMusic(float duration, scr_AudioClipWrapper wrapper){
		if(musicSource.clip != wrapper.clip){
			StartCoroutine(fadeBetweenMusic(duration,wrapper));
		}
	}

	public void startMusic(scr_AudioClipWrapper wrapper, float transition) {
		musicSource.clip = wrapper.clip;
		musicSource.volume = 0;
		musicSource.pitch = wrapper.pitch;
		musicSource.loop = wrapper.loop;
		musicSource.Play();
		StartCoroutine(fadeInStartMusic(transition, wrapper));
	}

	private IEnumerator fadeInStartMusic(float duration, scr_AudioClipWrapper wrapper){
		float volumeSpeed = wrapper.volume/duration, currentVolume = 0, delta, counter = 0;
		musicSource.volume = 0;
		musicSource.Play();

		while(counter < duration){
			delta = Time.unscaledDeltaTime;
			counter += delta;
			currentVolume += delta * volumeSpeed;

			musicSource.volume = currentVolume;
			yield return null;
		}

		musicSource.volume = wrapper.volume;

	}

	public void stopMusic() {
		if(musicSource.isPlaying) {
			musicSource.Stop();
		}
	}

	public void stopMusic(float timeToStop) {
		if(musicSource.isPlaying){
			StartCoroutine(fadeStopMusic(timeToStop));
		}
	}

	private IEnumerator fadeStopMusic(float duration) {
		float volumeSpeed = musicSource.volume/duration, currentVolume = musicSource.volume, delta, counter =0;
		while(counter < duration) {
			delta = Time.unscaledDeltaTime;
			counter += delta;
			currentVolume -= volumeSpeed * delta;
			musicSource.volume = currentVolume;
			yield return null;
		}
		musicSource.volume = 0;
		musicSource.Stop();
	}
	
	/// <summary>
	/// Realiza a transição entre duas músicas
	/// </summary>
	/// <param name="duration"></param>
	/// <param name="wrapper"></param>
	/// <returns></returns>
	private IEnumerator fadeBetweenMusic(float duration, scr_AudioClipWrapper wrapper){
		float volumeSpeed = 2*musicSource.volume/duration, currentVolume = musicSource.volume, delta, counter = 0;
		bool increasing = false;

		while(counter < duration){
			delta = Time.unscaledDeltaTime;
			counter += delta;
			if(!increasing) {
				currentVolume -= volumeSpeed * delta;
				if(currentVolume <= 0){
					increasing = true;
					musicSource.clip = wrapper.clip;
					musicSource.pitch = wrapper.pitch;
					musicSource.loop = wrapper.loop;
					volumeSpeed = 2*wrapper.volume/duration;
					currentVolume = 0;
					musicSource.Play();
				}
			} 
			else {
				currentVolume += volumeSpeed * delta;
			}
			musicSource.volume = currentVolume;
			yield return null;
		}

		musicSource.volume = wrapper.volume;

	}

}
