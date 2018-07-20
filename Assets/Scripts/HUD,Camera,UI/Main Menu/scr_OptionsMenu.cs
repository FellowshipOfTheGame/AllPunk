using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class scr_OptionsMenu : MonoBehaviour {

	#region varialbes

	[Header("AUDIO CONTROLLERS")]
	[SerializeField] Slider masterSlider;
	[SerializeField] Slider sfxSlider;
	[SerializeField] Slider voiceSlider;
	[SerializeField] Slider musicSlider;
	[SerializeField] AudioMixer audioMixer;

	string masterVolume = "masterVolume";
	string sfxVolume = "sfxVolume";
	string voiceVolume = "voiceVolume";
	string musicVolume = "musicVolume";

	[Header("GRAPHICS CONTROLLERS")]
	[SerializeField] Dropdown resolutionDropdown;
	[SerializeField] Toggle fullscreenToggle;
	[SerializeField] Dropdown qualityDropdown;
	Resolution[] resolutions;
	bool isFullscreen = true;
	int currResolutionIndex = 0;

	#endregion

	// Use this for initialization
	void Start () {
		setResolutionOptions ();
		setQualityOptions ();
		loadCurrentAudioVolume ();
	}

	#region
	public void setMasterVolume(float volume){
		audioMixer.SetFloat (masterVolume, volume);
	}
	public void setSfxVolume(float volume){
		audioMixer.SetFloat (sfxVolume, volume);
	}
	public void setVoiceVolume(float volume){
		audioMixer.SetFloat (voiceVolume, volume);
	}
	public void setMusicVolume(float volume){
		audioMixer.SetFloat (musicVolume, volume);
	}

	/// <summary>
	/// Loads the current audio volume. Used to set each of the sliders to the correct value
	/// </summary>
	void loadCurrentAudioVolume(){
		float value;
		audioMixer.GetFloat (masterVolume, out value);
		masterSlider.value = value;

		audioMixer.GetFloat (sfxVolume, out value);
		sfxSlider.value = value;

		audioMixer.GetFloat (voiceVolume, out value);
		voiceSlider.value = value;

		audioMixer.GetFloat (musicVolume, out value);
		musicSlider.value = value;
	}
	#endregion

	#region graphics methods

	/// <summary>
	/// Sets the fullscreen. Call back used by the fullscreen toggle
	/// </summary>
	/// <param name="fullscreen">If set to <c>true</c> fullscreen.</param>
	public void setFullscreen(bool fullscreen){
		isFullscreen = fullscreen;
		setResolution (currResolutionIndex);
	}

	/// <summary>
	/// Sets the overall graphics quality based on the default presets
	/// </summary>
	/// <param name="qualityIndex">Quality index.</param>
	public void setQuality(int qualityIndex){
		QualitySettings.SetQualityLevel (qualityIndex);
	}

	/// <summary>
	/// Selects the correct current quality options on the menu
	/// </summary>
	void setQualityOptions(){
		qualityDropdown.value = QualitySettings.GetQualityLevel();
		qualityDropdown.RefreshShownValue ();
	}

	/// <summary>
	/// Sets the resolution to a new one. Callback used by the Resolution dropdown
	/// </summary>
	/// <param name="resolutionIndex">Resolution index.</param>
	public void setResolution(int resolutionIndex){
		Screen.SetResolution (resolutions [resolutionIndex].width, resolutions [resolutionIndex].width, isFullscreen);
	}

	/// <summary>
	/// Sets the resolution options on the Dropdown menu
	/// </summary>
	void setResolutionOptions(){
		resolutions = Screen.resolutions;
		resolutionDropdown.ClearOptions();	
		List <string> resolutionStrings = new List<string>();

		for (int i = 0; i < resolutions.Length; i++) {
			resolutionStrings.Add (resolutions [i].width + "x" + resolutions [i].height);
			if (resolutions [i].width == Screen.currentResolution.width && resolutions [i].height == Screen.currentResolution.height) {
				currResolutionIndex = i;
			} 
		}
		resolutionDropdown.AddOptions (resolutionStrings);
		resolutionDropdown.value = currResolutionIndex;
		resolutionDropdown.RefreshShownValue ();
	}
	#endregion
}
