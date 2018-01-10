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

	//Variaveis salvadas
	public scr_Player_Stats playerStats;

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
		print(Application.persistentDataPath + "/PlayerStatus.dat");
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

	// Update is called once per frame
	void Update () {

		if (!isGameOver) {
			//Jogador morreu
			if (player == null) {
				endGame();
			}

			if (Input.GetButtonDown ("Cancel")) {
				pauseGame ();
			}


		}

		else {
			if(Input.anyKeyDown)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		
		if(Input.GetKey(KeyCode.X)) {
			Delete();
		}
	}
}
