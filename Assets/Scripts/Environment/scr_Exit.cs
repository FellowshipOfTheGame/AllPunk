﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Exit : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			GetComponentInParent<scr_GameManager> ().startGameOver ();
		}

	}

}
