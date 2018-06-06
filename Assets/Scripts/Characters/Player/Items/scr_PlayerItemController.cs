using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerItemController : MonoBehaviour {

	[SerializeField] List<scr_Item> playerItems = new List <scr_Item>();
	[SerializeField] scr_AudioClient audioClient;

	/// <summary>
	/// Adds the item.
	/// </summary>
	/// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
	/// <param name="newItem">New item.</param>
	public bool addItem(scr_Item newItem){

		//Should the player already have an item, increment its quantity
		foreach(scr_Item i in playerItems){
			if (i.GetType () == newItem.GetType()) {

				int newQty = i.getCurrQty () + 1;

				//Within maximum quantity
				if (newQty <= i.getMaxQty ()) {
					i.setCurrQty (newQty);
					return true;
				} else {
					//Exceeded Maximum quantity
					return false;
				}
			}
		}

		//print ("New item!");
		newItem.setPlayerReferences (this.gameObject);
		playerItems.Add (newItem);

		return true;
	}

	/// <summary>
	/// Gets a particular Item
	/// </summary>
	/// <returns>The item. Null if invalid index</returns>
	/// <param name="index">Index.</param>
	public scr_Item getItem (int index){
		scr_Item gotItem = null;
		try{
			gotItem = playerItems[index];
		}catch{
			gotItem = null;
		}
		return gotItem;
	}

	void Update(){
		///TEST INPUTS / FUNCTIONALITIES
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if(playerItems [0].useItem ())
				audioClient.playAudioClip ("Repair", scr_AudioClient.sources.sfx);
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			playerItems [1].useItem ();
		}
	}
}
