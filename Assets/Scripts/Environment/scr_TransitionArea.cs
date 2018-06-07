﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_TransitionArea : MonoBehaviour {

	[Tooltip("Cena para a qual o player será levado")]
	public string destinyScene;
	protected Collider2D interactionZone;

	private void Awake() {
		interactionZone = GetComponent<Collider2D> ();
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Player") {
			print("Trocou");
			scr_GameManager.instance.MoveToScene(destinyScene);
		}
	}
}