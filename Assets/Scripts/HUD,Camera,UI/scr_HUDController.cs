using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_HUDController : MonoBehaviour {

	#region variables
	public static scr_HUDController hudController;

	[Header("Player")]

	public GameObject player;
	//Incremento ou decremento no alfa da condensação
	private float condensationDelta = 0.001f;

	private scr_HealthController playerHealth;
	protected scr_PlayerEnergyController playerEnergy;
	private bool playerInSteam;

	[Header("Sliders and Text")]

	public Text healthText;
	public Slider healthSlider;

	public Text energyText;
	public Slider energySlider;

	public Text rightWeaponText;
	public Slider rightWeaponSlider;

	[Header("Images")]
	public Image condensationImg;

	#endregion


	void Awake () {
		if (hudController == null) {
			hudController = this;
			playerHealth = player.GetComponent<scr_HealthController> ();
			playerEnergy = player.GetComponent<scr_PlayerEnergyController> ();
			playerInSteam = false;
		} else if (hudController != this) {
			Destroy (this.gameObject);
		}

	}

	public void displayEndGameScreen(){
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}

		transform.Find ("endGameText").gameObject.SetActive(true);
	}


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

	void updatePlayerBars(){

		float playerMaxHp = playerHealth.getMaxHealth ();
		float playerCurrentHp = playerHealth.getCurrentHealth ();

		healthText.text = "Integrity " +
			((playerCurrentHp / playerMaxHp) * 100).ToString ("0.#") + "%";

		healthSlider.value = playerCurrentHp / playerMaxHp;


		float playerMaxEnergy = playerEnergy.getMaxEnergy ();
		float playerCurrentEnergy = playerEnergy.getCurrentEnergy ();

		energyText.text = "Energy " +
		((playerCurrentEnergy / playerMaxEnergy) * 100).ToString ("0.#") + "%";

		energySlider.value = playerCurrentEnergy / playerMaxEnergy;
	}



	// Update is called once per frame
	void Update () {
		//Se o player não morreu
		if (player != null) {
			updateWeaponTimers ();
			updatePlayerBars ();
		}

		if (playerInSteam && condensationImg.color.a < 1) {
			print ("Inc-ing");
			Color c = condensationImg.color;
			c.a+= condensationDelta * Time.deltaTime;
			condensationImg.color = c;


		} else if (!playerInSteam && condensationImg.color.a > 0) {
			print ("Dec-ing");
			Color c = condensationImg.color;
			c.a-= condensationDelta * Time.deltaTime;
			condensationImg.color = c;
		}
	}
		
	public void setPlayerInSteam(bool inSteam, float condensationDelta){
		playerInSteam = inSteam;
		this.condensationDelta = condensationDelta;
	}
}
