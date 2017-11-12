using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_SteamWall : MonoBehaviour {

	/*Quantidade de alpha que é adicionado à textura de condensação na tela, por frame*/
	public float condensationDelta = 1f;

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			scr_HUDController.hudController.setPlayerInSteam (true, condensationDelta);
			print ("pis");
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.tag == "Player") {
			scr_HUDController.hudController.setPlayerInSteam (false, condensationDelta);
			print ("pnis");
		}
	}
}
