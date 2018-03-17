using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
/// <summary>
/// Script for pickup items. Interacts withs the Player Item Controller and adds the new item to the inventory
/// </summary>
public class scr_PickupItem : MonoBehaviour {

	public scr_Item item;

	void OnTriggerStay (Collider col){
		//Player pegou
		if (col.tag == "Player") {

			scr_PlayerItemController ic = col.GetComponent<scr_PlayerItemController> ();

			if (ic.addItem (item)) {
				Destroy (this.gameObject);
			}
		}
	}

}
