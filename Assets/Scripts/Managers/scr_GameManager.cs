using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameManager : MonoBehaviour {
	
	#region variables

	public static scr_GameManager instance = null;

	public GameObject playerPrefab;

	public GameObject player;

	public bool fadeOutOnLoad = true;

	[Tooltip("Se houver algum outro sigma na cena durante o load, o jogo irá matar")]
	public bool killRemainingPlayerOnLoad = true;

	private GameObject exit;

	private bool isPause = true;
	private bool isGameOver;

	//Variaveis responsaveis por tratar a transição de cenas
	private string previusScene = "null";
	private bool isLoading = false;

    private scr_SaveManager saveManager;

	//Variaveis salvadas
	[SerializeField]
	public scr_Player_Stats playerStats;
	#endregion
		
	void Awake(){
		//Singleton Instanciation
		if(instance == null) {
			instance = this;
		}
		else if(instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);

		isPause = false;
		isGameOver = false;


        //Carrega dados salvos
        saveManager = new scr_SaveManager();
		playerStats = new scr_Player_Stats();
		bool result = Load();
		if(!result) {
			playerStats = new scr_Player_Stats();
			Save();
		}
		
		if(player == null) {
			player = GameObject.FindGameObjectWithTag("Player");
		}

		previusScene = "null";
		SceneManager.activeSceneChanged += OnSceneLoaded;
		//SceneManager.sceneLoaded += OnSceneLoaded;

	}

	// Update is called once per frame
	void Update () {
		/*
		if(Input.GetKeyDown(KeyCode.X)) {
			Delete();
		}

		if(Input.GetKeyDown(KeyCode.L)) {
			LoadGame();
		}
		*/
	}

	
	public void startGameOver(){
		scr_HUDController.hudController.displayEndGameScreen();
	}

	public void setPauseGame(bool pause){
		isPause = !pause;
        if (pause)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

		player.GetComponent<scr_PlayerController> ().enabled = isPause;
		player.GetComponent<scr_EPManager> ().pauseWeaponScripts (isPause);
	}

	#region Save/Load

/// <summary>
/// Método para salvar as informações carregadas em arquivo
/// </summary>
/// <returns>true=Salvou os dados no arquivo.false=Deu alguma merda</returns>
	public bool Save(){
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return false;
        }

        updatePlayerStats();

        saveManager.Save(playerStats);
		return true;
	}

	/// <summary>
	/// Método para carregar quais extras já foram desbloqueados
	/// </summary>
	/// <returns>true=Carregou os dados no arquivo	false=Arquivo não existente</returns>
	public bool Load(){

		//Verifica se o arquivo existe
		if (saveManager.hasSaveGame()) {
            playerStats = saveManager.Load();
			return true;
		} else
			return false;
	}

	/// <summary>
	/// Método para apagar dados salvos
	/// </summary>
	/// <returns>True: caso tenha sido apagado algum arquivo. Falso: caso não tenha sido encontrado o arquivo</returns>
	public bool Delete(){
        return saveManager.Delete();
	}

		/// <summary>
	/// Função que retorna se possui um save game ou não
	/// </summary>
	/// <returns>Se o jogo posssui save ou não</returns>
	public bool hasSaveGame(){
		if(playerStats.savePointScene.Equals("null"))
			return false;
		else
			return true;
	}

	/// <summary>
	/// Carrega o jogo, procurando nos arquivos a cena onde deve carregar
	/// É importante que a cena a ser carregada esteja nas cenas de build do jogo
	/// </summary>
	/// <returns>Retorna se o jogo conseguiu carregar uma cena</returns>
	public bool LoadGame() {
		if(playerStats.savePointScene == null) {
			Debug.Log("Can't find previous save");
			return false;
		}
		previusScene = "Load";
		string completePath = "Scenes/" + playerStats.savePointScene;
		isLoading = true;
		AsyncOperation aOp = SceneManager.LoadSceneAsync(completePath, LoadSceneMode.Single);

		return true;
	}

	#endregion

	#region MapTransition

		/// <summary>
	/// Função utilizada para mover o personagem entre cenas
	/// </summary>
	/// <param name="newScene">Cena para qual se deseja mover</param>
	public void MoveToScene(string newScene) {
		string previusScenePath = SceneManager.GetActiveScene().path;
		string[] separator = {"Scenes/", ".unity"};
		previusScene = previusScenePath.Split(separator, System.StringSplitOptions.None)[1];

		updatePlayerStats();
		string completePath = "Scenes/" + newScene;
		isLoading = true;
		AsyncOperation aOp = SceneManager.LoadSceneAsync(completePath, LoadSceneMode.Single);
		//SceneManager.LoadScene(completePath, LoadSceneMode.Single);
	}

	/// <summary>
	/// Função chamada na transição de mapas
	/// </summary>
	/// <param name="scene"></param>
	/// <param name="scene2"></param>
	private void OnSceneLoaded(Scene scene, Scene scene2) {

		//Fazer tocar a musica da cena
		GameObject sceneManager = GameObject.Find("SceneManager");
		scr_SceneManager sceneScript = null;
		if(sceneManager != null){
            sceneScript = sceneManager.GetComponent<scr_SceneManager>();
			makeMusicTransition(sceneScript);
		}

		//Fazer com que a tela inicial não apareça HUD
		if(scene2.name.Equals("MenuInicial")) {
			setHudVisible(false);
			return;
		}
		if(previusScene == "null")
			return;

		
        if (sceneManager == null)
        {
            print("Cant find Scene Manager");
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
			if(sceneScript == null)
           		sceneScript = sceneManager.GetComponent<scr_SceneManager>();

			Transform target = sceneScript.positionToSpawnInScene(previusScene, playerStats);

			if(target == null) {
				player = GameObject.FindGameObjectWithTag("Player");
			}
			if(target == null && player == null) {
				Debug.LogWarning("Can't locate or spawn player");
				return ;
			}
			if(target != null){
				//Kill other players that may be in scene
				if(killRemainingPlayerOnLoad){
					GameObject[] players;
					players = GameObject.FindGameObjectsWithTag("Player");
					foreach(GameObject g in players)
						Destroy(g);
				}

				player = spawnPlayerOnTransform(target);
			}
        }

		//Update instances
		isLoading = false;
		scr_Camera_Follow_Mouse cameraScript = Camera.main.GetComponent<scr_Camera_Follow_Mouse>();
		if(cameraScript != null) {
			cameraScript.player = player.transform;
		}
		//Legacy
		else {
			scr_CameraController oldController =  Camera.main.GetComponent<scr_CameraController>();
			if(oldController != null)
				oldController.player = player;
		}

		//Ajust camera position
		Vector3 playerPos = player.transform.position;
		playerPos.z = Camera.main.transform.position.z;
		Camera.main.transform.position = playerPos;

		setHudVisible(true);
		scr_HUDController.hudController.onLoadLevel();

		//Update know maps
		string mapName = SceneToMapName(scene2.path);
		playerStats.scenesDiscovered[mapName] = true;
		//Startminimap if allowed by scene script
		if(sceneScript != null && sceneScript.showMap)
		{
			generateMap(playerStats.scenesDiscovered, mapName);
			activateMiniMap(player.transform,mapName);
		}

		if(fadeOutOnLoad){
			setPauseGame(false);
			scr_HUDController.hudController.canPause = false;
			scr_HUDController.hudController.forceFadeOut(onFadeFinish);
		}
		else{
			setPauseGame(false);
		}
		print("Scene is Loaded");
	}

	public void makeMusicTransition(scr_SceneManager sceneScript) {
		if(sceneScript != null && sceneScript.musicClip != null){
				if(scr_AudioManager.instance.isPlayingMusic()){
					if(sceneScript.shouldTransitionMusic == scr_SceneManager.MusicMode.Transition)
						scr_AudioManager.instance.changeToMusic(sceneScript.musicTransitionTime,sceneScript.musicClip);
					else if (sceneScript.shouldTransitionMusic == scr_SceneManager.MusicMode.Stop){
						scr_AudioManager.instance.stopMusic(sceneScript.musicTransitionTime);
					}
				}
				else{
					if(sceneScript.shouldTransitionMusic == scr_SceneManager.MusicMode.Transition)
						scr_AudioManager.instance.startMusic(sceneScript.musicClip, sceneScript.musicTransitionTime);
					}
			}
	}

	private void onFadeFinish() {
		//setPauseGame(false);
		scr_HUDController.hudController.canPause = true;
	}

	/// <summary>
	/// Funçao utilizada para atualizar stats relacionados ao jogador, como vida e energia
	/// </summary>
	public void updatePlayerStats() {
		if(player == null)
			return ;
        //Atualizar stats de vida
		scr_HealthController health = player.GetComponent<scr_HealthController>();
		playerStats.maxHp = health.getMaxHealth();
		playerStats.currentHp = health.getCurrentHealth();
		playerStats.defense = health.defense;
		playerStats.poise = health.poise; 

		//Atualizar stats de energia
		scr_PlayerEnergyController energy = player.GetComponent<scr_PlayerEnergyController>();
		playerStats.maxResEnergy = energy.getMaxResEnergy();
		playerStats.currentResEnergy = energy.getCurrentResEnergy();
		playerStats.maxPrimEnergy = energy.getMaxPrimEnergy();
		playerStats.currentPrimEnergy = energy.getCurrentPrimEnergy();
		playerStats.rechargeRate = energy.getResRechargeRate();

		//Atualizar stats de armas desbloqueadas e equipadas
		scr_EPManager epman = player.GetComponent<scr_EPManager>();
		playerStats.unlockedEPs.Clear();
		if(epman == null)
			print("É o epman que está null");
		if(epman.getUnlockedParts() == null)
			print("É o dicionário");
		playerStats.unlockedEPs.CopyFrom(epman.getUnlockedParts());
		//playerStats.unlockedEPs.CopyFrom(epman.UnlockedEPs);
		playerStats.leftWeaponEquiped = epman.getCurrentPart(scr_EP.EpType.Arm, scr_EPManager.ArmToEquip.LeftArm);
		playerStats.rightWeaponEquiped = epman.getCurrentPart(scr_EP.EpType.Arm, scr_EPManager.ArmToEquip.RightArm);
		playerStats.headEquiped = epman.getCurrentPart(scr_EP.EpType.Head);
		playerStats.torsoEquiped = epman.getCurrentPart(scr_EP.EpType.Torso);
		playerStats.legsEquiped = epman.getCurrentPart(scr_EP.EpType.Legs);
		
	}


	private GameObject spawnPlayerOnTransform(Transform target) {
		GameObject toSpawn = GameObject.Instantiate(playerPrefab, target.position, target.rotation);
		
		//Atualizar stats de vida
		scr_HealthController health = toSpawn.GetComponent<scr_HealthController>();
		health.maxHp =playerStats.maxHp;
		health.setCurrentHealth(playerStats.currentHp);
		health.defense = playerStats.defense;
		health.poise = playerStats.poise;

		//Atualizar stats de energia
		scr_PlayerEnergyController energy = toSpawn.GetComponent<scr_PlayerEnergyController>();
		energy.setMaxPrimEnergy(playerStats.maxPrimEnergy);
		energy.setCurrentPrimEnergy(playerStats.currentPrimEnergy);
		energy.setMaxResEnergy(playerStats.maxResEnergy);
		energy.setCurrentResEnergy(playerStats.currentResEnergy);
		energy.setResRechargeRate(playerStats.rechargeRate);


		//Atualizar stats de armas desbloqueadas e equipadas
		scr_EPManager epman = toSpawn.GetComponent<scr_EPManager>();
		epman.applyPlayerStats(playerStats);

		return toSpawn;
	}

	public void goToMainMenu() {
		MoveToScene("MenuInicial");
		previusScene = "null";
	}

	public void setHudVisible(bool visible){
		Transform find = instance.transform.Find("hud_Canvas");
		if(find != null)
			find.gameObject.SetActive(visible);
	}

	public void generateMap(StringBoleanDictionary discoveredScenes, string currentScene)
	{
		MapUICreator mapCreator = GameObject.FindObjectOfType<MapUICreator>();
		if(mapCreator)
			mapCreator.Generate(discoveredScenes, currentScene);
	}

	public void activateMiniMap(Transform playerTarget, string currentScene)
	{
		Minimap minimap = GameObject.FindObjectOfType<Minimap>();
		if(minimap)
			minimap.StartMinimap(playerTarget,currentScene);
	}

	public void HideMaps()
	{
		MapUICreator mapCreator = GameObject.FindObjectOfType<MapUICreator>();
		if(mapCreator)
			mapCreator.PanelActive = false;

		Minimap minimap = GameObject.FindObjectOfType<Minimap>();
		if(minimap)
			minimap.StopMinimap();
	}

	private string SceneToMapName(string path)
    {
        string[] parts = path.Split('.');
        return parts[0].Remove(0,14); //Remove initial part, to be equal to what we use on the scripts
    }


	#endregion

}
