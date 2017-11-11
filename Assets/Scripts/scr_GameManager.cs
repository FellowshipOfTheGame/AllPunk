using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameManager : MonoBehaviour {
	
	#region variables

	public GameObject player;

	public GameObject exit;

	private bool isPause;
	private bool isGameOver;

	public static scr_GameManager gameManager;
	#endregion

	//Implementa SINGLETON
	void Awake(){
		if (gameManager == null) {
			gameManager = this;

			isPause = false;
			isGameOver = false;

			if(exit == null)
				exit = transform.Find ("trg_Exit").gameObject;

		} else if (gameManager != this) {
			Destroy (this.gameObject);
		}
	}


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
		
	}
}
