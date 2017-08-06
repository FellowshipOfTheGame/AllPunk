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
			print ("pause");
			isPause = !isPause;
			Time.timeScale = System.Convert.ToSingle(isPause);
		}
	}
}
