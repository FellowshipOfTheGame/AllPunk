using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Interactable : MonoBehaviour {

	Collider2D interactionZone;

	// Use this for initialization
	void Start () {
		interactionZone = GetComponent<Collider2D> ();
	}
	
	void OnTriggerEnter2D(Collider2D col){
		//Habilita Texto
	}

	void OnTriggerExit2D(Collider2D col){
		//Habilita Texto
	}
}
