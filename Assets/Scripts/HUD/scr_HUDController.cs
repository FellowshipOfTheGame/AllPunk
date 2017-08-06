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

	private scr_HealthController playerHealthScr;


	void updateWeaponTimers(){
		Vector4 timers = player.GetComponent<scr_PA_Manager>().getCountdownTimers();
		if (timers.x != 0 && timers.y != 0) {
			float dirTime = timers.x / timers.y;// Tempo atual:total
			rightWeaponText.text = "RCountDown" + dirTime.ToString ("F2");
		} else
			rightWeaponText.text = "";

		if (timers.z != 0 && timers.w != 0) {
			float dirTime = timers.z / timers.w;// Tempo atual:total
			//rightWeaponText.text = "RCountDown" + dirTime.ToString ("F2");
		}
	}

	void Awake () {
		playerHealthScr = player.GetComponent<scr_HealthController> ();

	}
	
	// Update is called once per frame
	void Update () {
		updateWeaponTimers ();

		float playerMaxHp = playerHealthScr.getMaxHealth ();
		float playerCurrentHp = playerHealthScr.getCurrentHealth ();
			
		healthText.text = "Integrity " + 
			(playerCurrentHp/playerMaxHp)*100 + "%";

		healthSlider.value = playerCurrentHp / playerMaxHp;

	}
}
