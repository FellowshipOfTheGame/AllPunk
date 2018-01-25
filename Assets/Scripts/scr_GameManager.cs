using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Bibliotecas para escrever e apagar arquivos
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class scr_GameManager : MonoBehaviour {
	
	#region variables

	public static scr_GameManager instance = null;

	public GameObject player;

	private GameObject exit;
	public bool DEBUG_THEREISAEXIT = true;

	private bool isPause;
	private bool isGameOver;

	//Variaveis responsaveis por tratar a transição de cenas
	private string previusScene = "Null";
	private bool isLoading = false;

	//Variaveis salvadas
	public scr_Player_Stats playerStats;
	#endregion

	public void endGame(){		
		GetComponentInChildren<scr_HUDController> ().displayEndGameScreen ();
		isGameOver = true;
	}

	public void pauseGame(){
		isPause = !isPause;
		Time.timeScale = System.Convert.ToSingle (isPause);

		player.GetComponent<scr_PlayerController> ().enabled = isPause;
		player.GetComponent<scr_PA_Manager> ().pauseWeaponScripts (isPause);
	}
		

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
		bool result = Load();
		if(!result) {
			playerStats = new scr_Player_Stats();
			Save();
		}

	}

/// <summary>
/// Método para salvar as informações carregadas em arquivo
/// </summary>
/// <returns>true=Salvou os dados no arquivo.false=Deu alguma merda</returns>
	public bool Save(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/PlayerStatus.dat");

		bf.Serialize (file, playerStats);

		file.Close ();

		print("Salvo");
		return true;
	}

	/// <summary>
	/// Método para carregar quais extras já foram desbloqueados
	/// </summary>
	/// <returns>true=Carregou os dados no arquivo	false=Arquivo não existente</returns>
	public bool Load(){

		//Verifica se o arquivo existe
		if (File.Exists (Application.persistentDataPath + "/PlayerStatus.dat")) {

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/PlayerStatus.dat", FileMode.Open);

			playerStats = (scr_Player_Stats) bf.Deserialize (file);
			file.Close ();
			return true;
		} else
			return false;
	}

	/// <summary>
	/// Método para apagar dados salvos
	/// </summary>
	/// <returns>True: caso tenha sido apagado algum arquivo. Falso: caso não tenha sido encontrado o arquivo</returns>
	public bool Delete(){
		if (File.Exists (Application.persistentDataPath + "/PlayerStatus.dat")) {
			File.Delete (Application.persistentDataPath + "/PlayerStatus.dat");
			print ("All deleted");
			playerStats = new scr_Player_Stats();
			return true;
		} else
			return false;
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
		playerStats.maxEnergy = energy.getMaxEnergy();
		playerStats.currentEnergy = energy.getCurrentEnergy();
		playerStats.rechargeRate = energy.rechargeRate;
	}

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
		StartCoroutine(WaitSceneLoad(aOp));
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
		StartCoroutine(WaitSceneLoad(aOp));

		return true;
	}

	
	/// <summary>
	/// Corrotina para realizar as ações de carregamento após o mapa ter sido carregado.
	/// Foi necessário isso pois o loading termina apenas no frame depois de iniciar o carregamento.
	/// O load é assincrono, o que significa que há uma liberdade de um pequeno intervalo em que o jogador
	/// não pode ser morto, mas pode ser ferido
	/// </summary>
	/// <param name="waitOperation">Tag da operação de carregamento da cena</param>
	/// <returns></returns>
	private IEnumerator WaitSceneLoad(AsyncOperation waitOperation) {
		print("Loading Scene");

		while(!waitOperation.isDone)
			yield return null;

		GameObject sceneManager = GameObject.Find("SceneManager");
		if(sceneManager == null) {
			print("Cant find Scene Manager");
		}
		scr_SceneManager sceneScript = sceneManager.GetComponent<scr_SceneManager>();

		GameObject spawnPlayer = sceneScript.spawnPlayerFromScene(previusScene, playerStats);
		player = spawnPlayer;

		//Update instances
		isLoading = false;
		scr_CameraController cameraScript = Camera.main.GetComponent<scr_CameraController>();
		cameraScript.player = player;

		//Ajust camera position
		Vector3 playerPos = player.transform.position;
		playerPos.z = cameraScript.transform.position.z;
		cameraScript.transform.position = playerPos;
		print("Scene is Loaded");
		
	}

	// Update is called once per frame
	void Update () {

		if (!isGameOver) {
			//Jogador morreu. Ele não pode morrer enquanto o jogo está carregando
			if (!isLoading && player == null) {
				endGame();
			}

			if (Input.GetButtonDown ("Cancel")) {
				pauseGame ();
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
}
