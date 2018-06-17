using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameManager : MonoBehaviour {
	
	#region variables

	public static scr_GameManager instance = null;

	public GameObject playerPrefab;

	public GameObject player;

    public bool canPause = true;

	private GameObject exit;
	public bool DEBUG_THEREISAEXIT = true;

	private bool isPause = true;
	private bool isGameOver;

	//Variaveis responsaveis por tratar a transição de cenas
	private string previusScene = "Null";
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

		if(DEBUG_THEREISAEXIT)
			exit = transform.Find ("trg_Exit").gameObject;

        //Carrega dados salvos
        saveManager = new scr_SaveManager();
		playerStats = new scr_Player_Stats();
		bool result = Load();
		if(!result) {
			playerStats = new scr_Player_Stats();
			Save();
		}
		

		previusScene = "null";
		SceneManager.activeSceneChanged += OnSceneLoaded;
		//SceneManager.sceneLoaded += OnSceneLoaded;

	}

	// Update is called once per frame
	void Update () {

		if (!isGameOver) {
			//Jogador morreu. Ele não pode morrer enquanto o jogo está carregando
			if (!isLoading && player == null) {
				endGame();
			}

			if (Input.GetButtonDown ("Cancel") && canPause) {
				setPauseGame (isPause);
			}


		}

		else {
			if(Input.anyKeyDown){
				//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				isGameOver = false;
				LoadGame();
				GetComponentInChildren<scr_HUDController> ().hideEndGameScreen();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.X)) {
			Delete();
		}

		if(Input.GetKeyDown(KeyCode.L)) {
			LoadGame();
		}
	}

	public void endGame(){		
		GetComponentInChildren<scr_HUDController> ().displayEndGameScreen ();
		isGameOver = true;
        Delete();
        AsyncOperation aOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

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
		previusScene = SceneManager.GetActiveScene().name;
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

		if(previusScene == "null")
			return;

		GameObject sceneManager = GameObject.Find("SceneManager");
        if (sceneManager == null)
        {
            print("Cant find Scene Manager");
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            scr_SceneManager sceneScript = sceneManager.GetComponent<scr_SceneManager>();

			Transform target = sceneScript.positionToSpawnInScene(previusScene, playerStats);
            player = spawnPlayerOnTransform(target);
        }

		//Update instances
		isLoading = false;
		scr_CameraController cameraScript = Camera.main.GetComponent<scr_CameraController>();
        cameraScript = Camera.main.GetComponent<scr_CameraController>();
		cameraScript.player = player;

		//Ajust camera position
		Vector3 playerPos = player.transform.position;
		playerPos.z = cameraScript.transform.position.z;
		cameraScript.transform.position = playerPos;
		print("Scene is Loaded");
	}

	/// <summary>
	/// Funçao utilizada para atualizar stats relacionados ao jogador, como vida e energia
	/// </summary>
	public void updatePlayerStats() {
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
		energy.currPrimEnergy = playerStats.currentPrimEnergy;
		energy.setMaxResEnergy(playerStats.maxResEnergy);
		energy.setCurrentResEnergy(playerStats.currentResEnergy);
		energy.setResRechargeRate(playerStats.rechargeRate);


		//Atualizar stats de armas desbloqueadas e equipadas
		scr_EPManager epman = toSpawn.GetComponent<scr_EPManager>();
		epman.applyPlayerStats(playerStats);

		return toSpawn;
	}

	#endregion

}
