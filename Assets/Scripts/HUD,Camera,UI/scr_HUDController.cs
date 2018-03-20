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
	private scr_PlayerItemController playerItems;
	private bool playerInSteam;

	[Header("Sliders and Text")]

	public Text healthText;
	public Slider healthSlider;

	public Text energyText;
	public Slider energySlider;

	public Text rightWeaponText;
	public Slider rightWeaponSlider;

	public Text item1Text;
	public Text item2Text;

	[Header("Images")]
	public Image condensationImg;

	#endregion


	void Awake () {
		if (hudController == null) {
			hudController = this;
			if(player == null)
				player = scr_GameManager.instance.player;
			playerHealth = player.GetComponent<scr_HealthController> ();
			playerEnergy = player.GetComponent<scr_PlayerEnergyController> ();
			playerItems = player.GetComponent<scr_PlayerItemController> ();
			playerInSteam = false;

			//Seta o pai da imagem para o jogador - dessa forma a imagem sempre seguirá o jogador
			//condensationImg.transform.SetParent (player.transform);
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

	public void hideEndGameScreen(){
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
		}

		transform.Find ("endGameText").gameObject.SetActive(false);
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


	void updatePlayerItems(){
		scr_Item item1 = playerItems.getItem (0);
		if (item1 != null) {
			int item1Curr = item1.getCurrQty ();
			int item1Max = item1.getMaxQty ();
			item1Text.text = item1Curr + "/" + item1Max;
		}
		else
			item1Text.text = "N/A";
		
		scr_Item item2 = playerItems.getItem (1);
		if (item2 != null) {
			int item2Curr = playerItems.getItem (1).getCurrQty ();
			int item2Max = playerItems.getItem (1).getMaxQty ();
			item2Text.text = item2Curr + "/" + item2Max;
		}
		else
			item2Text.text = "N/A";

	}


	// Update is called once per frame
	void Update () {
		//Se o player não morreu
		if (player != null) {
			updateWeaponTimers ();
			updatePlayerBars ();
			updatePlayerItems ();
		} else {
			//Tenta recuperar referencia perdida
			recoverPlayerReference();
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

		//Verificação de equipamento, se tiver os goggles, sempre falso

		playerInSteam = inSteam;
		this.condensationDelta = condensationDelta;
	}

	/// <summary>
	/// Função utilizada para quando a HUD não consegue achar um player
	/// </summary>
	private void recoverPlayerReference() {
		player = scr_GameManager.instance.player;
		if(player != null) {
			playerHealth = player.GetComponent<scr_HealthController> ();
			playerEnergy = player.GetComponent<scr_PlayerEnergyController> ();
            //updateWeaponTimers();
            updatePlayerBars();
        }
	}
}
