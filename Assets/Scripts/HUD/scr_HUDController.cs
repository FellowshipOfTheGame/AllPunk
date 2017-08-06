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

	void Awake () {
		playerHealthScr = player.GetComponent<scr_HealthController> ();

	}
	
	// Update is called once per frame
	void Update () {
		float playerMaxHp = playerHealthScr.getMaxHealth ();
		float playerCurrentHp = playerHealthScr.getCurrentHealth ();
			
		healthText.text = "Integrity " + 
			(playerCurrentHp/playerMaxHp)*100 + "%";

		healthSlider.value = playerCurrentHp / playerMaxHp;
	}
}
