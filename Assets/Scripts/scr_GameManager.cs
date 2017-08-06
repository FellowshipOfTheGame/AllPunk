using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_GameManager : MonoBehaviour {

	public GameObject player;

	private bool isPause;

	void Awake(){
		isPause = false;
	}

	// Update is called once per frame
	void Update () {
		//Jogador morreu
		if (player == null) {
		}

		if (Input.GetButtonDown ("Cancel")) {
			isPause = !isPause;
			Time.timeScale = System.Convert.ToSingle(isPause);

			player.GetComponent<scr_PlayerController> ().enabled = isPause;
			player.GetComponent<scr_PA_Manager> ().pauseWeaponScripts(isPause);
		}
	}
}
