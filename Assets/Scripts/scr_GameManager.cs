using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameManager : MonoBehaviour {

	public GameObject player;

	private GameObject exit;

	private bool isPause;
	private bool isGameOver;



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
		isPause = false;
		isGameOver = false;

		exit = transform.Find ("trg_Exit").gameObject;
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
