using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_HUDController : MonoBehaviour {

	public GameObject player;

	public Text healthText;
	public Slider healthSlider;

	public Text energyText;
	public Slider energySlider;

	public Text rightWeaponText;
	public Slider rightWeaponSlider;

	private scr_HealthController playerHealthScr;


	void updateWeaponTimers(){
		Vector4 timers = player.GetComponent<scr_PA_Manager>().getCountdownTimers();
		if (timers.x != 0 && timers.y != 0) {
			rightWeaponSlider.GetComponentInParent<CanvasGroup> ().alpha = 1;

			float rightTime = timers.x / timers.y;// Tempo atual:total
			rightWeaponText.text = "RCountDown" + rightTime.ToString ("F2");
			rightWeaponSlider.value = rightTime;
		} else {

			rightWeaponText.text = "";
			rightWeaponSlider.value = 0;

			rightWeaponSlider.GetComponentInParent<CanvasGroup> ().alpha = 0;
		}

		if (timers.z != 0 && timers.w != 0) {
			//float leftTime = timers.z / timers.w;// Tempo atual:total
		}
	}

	void Awake () {
		playerHealthScr = player.GetComponent<scr_HealthController> ();

	}
	
	// Update is called once per frame
	void Update () {
		//Se o player não morreu
		if (player != null) {
			updateWeaponTimers ();

			float playerMaxHp = playerHealthScr.getMaxHealth ();
			float playerCurrentHp = playerHealthScr.getCurrentHealth ();
			
			healthText.text = "Integrity " +
			(playerCurrentHp / playerMaxHp) * 100 + "%";

			healthSlider.value = playerCurrentHp / playerMaxHp;
		}

	}
}
