using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_OptionsMenu : MonoBehaviour {

	[Header("GRAPHICS CONTROLLERS")]
	[SerializeField] Dropdown resolutionDropdown;
	[SerializeField] Toggle fullscreenToggle;
	[SerializeField] Dropdown qualityDropdown;
	Resolution[] resolutions;
	bool isFullscreen = true;
	int currResolutionIndex = 0;

	// Use this for initialization
	void Start () {
		setResolutionOptions ();
	}

	public void setFullscreen(bool fullscreen){
		isFullscreen = fullscreen;
		setResolution (currResolutionIndex);
	}

	public void setQuality(int qualityIndex){
		QualitySettings.SetQualityLevel (qualityIndex);
	}

	void setQualityOptions(){
		qualityDropdown.value = QualitySettings.GetQualityLevel();
		qualityDropdown.RefreshShownValue ();
	}

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
}
