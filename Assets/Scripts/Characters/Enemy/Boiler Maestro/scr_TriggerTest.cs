using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_TriggerTest : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if(col.CompareTag("Player"))
			print ("TriggerEntrou");
		/*if(col.IsTouchingLayers(LayerMask.GetMask("Player")))
			print ("TriggerEntrou");*/
	}

	void OnTriggerStay2D(Collider2D col){
		if(col.CompareTag("Player"))
			print ("Ficou");
		/*if(col.IsTouchingLayers(LayerMask.GetMask("Player")))
			print ("TriggerFicou");*/
	}

	void OnTriggerExit2D(Collider2D col){
		if(col.CompareTag("Player"))
			print ("TriggerVazou");
		/*if(col.IsTouchingLayers(LayerMask.GetMask("Player")))
			print ("TriggerSaiu");*/
	}
}
