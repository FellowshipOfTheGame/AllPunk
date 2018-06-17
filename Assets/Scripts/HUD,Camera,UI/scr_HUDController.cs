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
	private scr_PlayerItemController playerItemsScript;
	private List<scr_Item> playerItemList;
	private bool playerInSteam;

	[Header("Health")]

	public Text healthText;
	public Slider healthSlider;
	[Header("Energy")]

	public Text primEnergyText;
	public Slider primEnergySlider;

	public Text resEnergyText;
	public Slider resEnergySlider;
	[Header("Weapons")]

	public Text rightWeaponText;
	public Slider rightWeaponSlider;
	[Header("Items")]

	public Text [] itemTexts;
	public Image[] itemImages;
	[Header("Condensation")]
	public Image condensationImg;

	#endregion

	#region Monobehavior Methods
	void Awake () {
		if (hudController == null) {
			hudController = this;
			if(player == null)
				player = scr_GameManager.instance.player;
			playerHealth = player.GetComponent<scr_HealthController> ();
			playerEnergy = player.GetComponent<scr_PlayerEnergyController> ();
			playerItemsScript = player.GetComponent<scr_PlayerItemController> ();
			playerInSteam = false;

			//Seta o pai da imagem para o jogador - dessa forma a imagem sempre seguirá o jogador
			//condensationImg.transform.SetParent (player.transform);
		} else if (hudController != this) {
			Destroy (this.gameObject);
		}

	}

	void Start(){
		playerEnergy.addEnergyChangeCallback (updateEnergyBars);
		playerHealth.addHealthChangeCallback (updateHealthBar);
		playerItemsScript.addItemChangeCallback (updateItems);
		playerItemList = playerItemsScript.getAllItems ();
	}

	// Update is called once per frame
	void Update () {
		//Se o player não morreu
		if (player != null) {
			updateWeaponTimers ();
		} else {
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

	#endregion

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
		/*Vector4 timers = player.GetComponent<scr_PA_Manager>().getCountdownTimers();
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
		}*/
	}

	/// <summary>
	/// Invoked by the player's Health to Update the health bar.
	/// </summary>
	void updateHealthBar(){

		float playerMaxHp = playerHealth.getMaxHealth ();
		float playerCurrentHp = playerHealth.getCurrentHealth ();

		healthText.text = "Integrity " +
			((playerCurrentHp / playerMaxHp) * 100).ToString ("0.#") + "%";

		healthSlider.value = playerCurrentHp / playerMaxHp;
	}

	/// <summary>
	/// Invoked by the player's Energy to update Energy bars
	/// </summary>
	void updateEnergyBars(){
		float playerMaxResEnergy = playerEnergy.getMaxResEnergy ();
		float playerCurrentResEnergy = playerEnergy.getCurrentResEnergy ();

		float playerMaxPrimEnergy = playerEnergy.getMaxResEnergy ();
		float playerCurrentPrimEnergy = playerEnergy.getCurrentResEnergy ();

		resEnergyText.text = "Reserve " +
			((playerCurrentResEnergy / playerMaxResEnergy) * 100).ToString ("0.#") + "%";
		resEnergySlider.value = playerCurrentResEnergy / playerMaxResEnergy;

		primEnergyText.text = "Primary " + 
			((playerCurrentPrimEnergy / playerMaxPrimEnergy) * 100).ToString ("0.#") + "%";
		primEnergySlider.value = playerCurrentPrimEnergy / playerMaxPrimEnergy;
	}


	/// <summary>
	/// Invoked by the player's Energy to update the item UI Quantities
	/// </summary>
	void updateItems(){
		int index = 0;
		foreach (scr_Item item in playerItemList) {
			if (item != null) {
				itemImages [index].sprite = item.getItemSprite();
				itemTexts [index].text = item.getCurrQty () + "/" + item.getMaxQty ();
			} else
				itemTexts [index].text = "N/A";
			index++;
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
			updateHealthBar();
        }
	}
}
