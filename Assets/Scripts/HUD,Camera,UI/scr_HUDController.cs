using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
	private bool playerHasGoogles = false;

	[Header("Fade")]
	public RawImage fadeImg;
	public float fadeDuration = 1f;
	private bool isFading = false;
	private UnityEvent fadeCallback;

	[Header("Pause")]
	public GameObject pausePanel;
	public float alphaWhenPaused = 0.6f;
	public float pausedTransition = 0.5f;
	public bool canPause = true;
	private bool isPaused = false;

	[Header("GameOver")]
	public GameObject gameOverPanel;
	public Text[] gameOverText;
	public Button gameOverLoadButton;
	public float timeToFadeGameOverScreen;
	public float timeToAppearGameOverText;
	public Color textColor;
	private bool isGameOver;

	#endregion

	#region Monobehavior Methods
	void Awake () {
		if (hudController == null) {
			hudController = this;
			playerInSteam = false;

			fadeCallback = new UnityEvent();
			//Seta o pai da imagem para o jogador - dessa forma a imagem sempre seguirá o jogador
			//condensationImg.transform.SetParent (player.transform);
		} else if (hudController != this) {
			Destroy (this.gameObject);
		}

	}

	void Start(){
		recoverPlayerReference();
		
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

		//Pause menu
		if(canPause && Input.GetButtonDown ("Cancel")) {
			if(!isPaused){
				scr_GameManager.instance.setPauseGame(true);
				pausePanel.SetActive(true);
				isFading = true;
				StartCoroutine(fadeTo(alphaWhenPaused,pausedTransition));
				isPaused = true;
			}
			else {
				onClickResume();
			}
		}
	}

	#endregion

	public void displayEndGameScreen(){
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}

		fadeImg.gameObject.SetActive(true);

		canPause = false;
		isFading = true;
		isGameOver = true;
		fadeCallback.AddListener(showGameOverText);
		StartCoroutine(fadeTo(1,timeToFadeGameOverScreen));
	}

	private void showGameOverText(){
		gameOverPanel.SetActive(true);
		StartCoroutine(changeGameOverText());
		gameOverLoadButton.interactable = scr_GameManager.instance.hasSaveGame();
	}

	private IEnumerator changeGameOverText(){
		float delta, counter = 0, speed = 1/timeToAppearGameOverText;
		Color color = textColor;
		color.a = 0;
		while (counter < timeToAppearGameOverText) {
			delta = Time.unscaledDeltaTime;
			color.a += delta * speed;
			counter += delta;
			foreach(Text t in gameOverText){
				t.color = color;
			}

			yield return null;
		}
		color.a = 1;
		foreach(Text t in gameOverText){
			t.color = color;
		}

	}

	public void hideEndGameScreen(){
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
		}

		pausePanel.SetActive(false);
		gameOverPanel.SetActive(false);
		isGameOver = false;
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

		float playerMaxPrimEnergy = playerEnergy.getMaxPrimEnergy ();
		float playerCurrentPrimEnergy = playerEnergy.getCurrentPrimEnergy ();

		resEnergyText.text = "Reserve " +
			((playerCurrentResEnergy / playerMaxResEnergy) * 100).ToString ("0.#") + "%";
		resEnergySlider.value = playerCurrentResEnergy / playerMaxResEnergy;

		if (playerMaxPrimEnergy != 0) {
			primEnergyText.text = "Primary " +
			((playerCurrentPrimEnergy / playerMaxPrimEnergy) * 100).ToString ("0.#") + "%";
			primEnergySlider.value = playerCurrentPrimEnergy / playerMaxPrimEnergy;
		} else {
			primEnergyText.text = "Not Equipped ";
			primEnergySlider.value = 0;
		}
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
		if(!inSteam || (inSteam && !playerHasGoogles)){
			playerInSteam = inSteam;
			this.condensationDelta = condensationDelta;
		}
	}

	public void setPlayerHasGoggles(bool hasGoggles){
		playerHasGoogles = hasGoggles;
		if(playerInSteam && playerHasGoogles)
			playerInSteam = false;
	}

	/// <summary>
	/// Realiza um fade in na hud e chama a função enviada quando acabar
	/// </summary>
	/// <param name="callback">Método a ser chamado quando acabar o fade in</param>
	public void fadeIn(UnityAction callback) {
		if(!isFading){
			isFading = true;
			//fadeImg.color = new Color(0,0,0,0);
			if(callback != null && fadeCallback != null)
				fadeCallback.AddListener(callback);
			StartCoroutine(fadeTo(1,fadeDuration));
		}
	}

	/// <summary>
	/// Realiza um fade out na hud e chama a função enviada quando acabar
	/// </summary>
	/// <param name="callback">Método a ser chamado quando acabar o fade out</param>
	public void fadeOut(UnityAction callback) {
		if(!isFading) {
			isFading = true;
			if(callback != null && fadeCallback != null)
				fadeCallback.AddListener(callback);
			StartCoroutine(fadeTo(0,fadeDuration));
		}
	}

	/// <summary>
	/// Realiza um fade out começando do preto na hud e chama a função enviada quando acabar
	/// </summary>
	/// <param name="callback">Método a ser chamado quando acabar o fade out</param>
	public void forceFadeOut(UnityAction callback) {
		if(!isFading) {
			isFading = true;
			fadeImg.color = new Color(0,0,0,1);
			if(callback != null && fadeCallback != null)
				fadeCallback.AddListener(callback);
			StartCoroutine(fadeTo(0,fadeDuration));
		}
	}

	public void onLoadLevel(){
		recoverPlayerReference();
		playerInSteam = false;
	}

	#region ButtonFunctions

	public void onClickResume(){
		scr_GameManager.instance.setPauseGame(false);
		pausePanel.SetActive(false);
		isFading = true;
		isPaused = false;
		StartCoroutine(fadeTo(0,pausedTransition));
	}

	public void onClickLoad(){
		if(isGameOver){
			hideEndGameScreen();
		}

		scr_GameManager.instance.LoadGame();

	}

	public void onClickExit(){
		if(isGameOver){
			hideEndGameScreen();
		}

		scr_GameManager.instance.goToMainMenu();
	}

	#endregion

	/// <summary>
	/// Função utilizada para quando a HUD não consegue achar um player
	/// </summary>
	private void recoverPlayerReference() {
		player = scr_GameManager.instance.player;
		if(player != null) {
			playerHealth = player.GetComponent<scr_HealthController> ();
			playerEnergy = player.GetComponent<scr_PlayerEnergyController> ();
			playerItemsScript = player.GetComponent<scr_PlayerItemController> ();
			updateHealthBar();
			playerEnergy.addEnergyChangeCallback (updateEnergyBars);
			playerHealth.addHealthChangeCallback (updateHealthBar);
			playerItemsScript.addItemChangeCallback (updateItems);
			playerItemList = playerItemsScript.getAllItems ();
        }
	}

	private IEnumerator fadeTo(float finalAlpha, float duration) {
		float deltaFade = (finalAlpha - fadeImg.color.a)/duration;
		Color color = fadeImg.color;
		float counter = 0, delta;
		while(counter < duration){
			delta = Time.unscaledDeltaTime;
			color.a += delta * deltaFade;
			counter += delta;
			fadeImg.color = color;
			yield return null;
		}
		color.a = finalAlpha;
		isFading = false;
		if(fadeCallback != null){
			fadeCallback.Invoke();
			fadeCallback.RemoveAllListeners();
		}
	}
}
