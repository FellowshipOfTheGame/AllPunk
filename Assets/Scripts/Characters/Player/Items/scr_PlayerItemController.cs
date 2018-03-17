using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerItemController : MonoBehaviour {

	List<scr_Item> playerItems = new List <scr_Item>();

	/// <summary>
	/// Adds an item
	/// </summary>
	/// <param name="item">Item.</param>
	public bool addItem(scr_Item newItem){

		//Should the player already have an item, increment its quantity
		foreach(scr_Item i in playerItems){
			if (i.GetType () == newItem.GetType()) {
				i.setCurrQty (i.getCurrQty() + 1);
				return true;
			}
		}

		newItem.setPlayerReferences (this.gameObject);
		playerItems.Add (newItem);

		return false;
	}

	///ADD ITEM = ADICIONA O SCRIPT, SE JA TIVER ADICIONA UM NA QUANTIDADE


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


}
