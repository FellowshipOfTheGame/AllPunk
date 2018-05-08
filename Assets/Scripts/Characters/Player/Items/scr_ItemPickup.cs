using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
/// <summary>
///  Script for pickup items. Interacts withs the Player Item Controller and adds the new item to the inventory
/// </summary>
public class scr_ItemPickup : MonoBehaviour {
	/// <summary>
	/// The item to be added. Although listed as a Monobehaviour here, it must implement the scr_Item interface for it to function with the item manager
	/// </summary>
	public MonoBehaviour item;


	void OnTriggerStay2D (Collider2D col){

		if (col.tag == "Player") {
			scr_PlayerItemController ic = col.GetComponent<scr_PlayerItemController> ();

			//If the item indeed implements the interface
			if (item is scr_Item) {
				//Cast as scr_Item for polymorphism
				scr_Item isItem = (scr_Item)item;

				if (ic.addItem (isItem)) {
					Destroy (this.gameObject);
				}
			} else {
				Debug.LogError ("Selected Item Does not implement scr_Item");
			}
		}
	}

}
