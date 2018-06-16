using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class scr_PlayerItemController : MonoBehaviour {

	[SerializeField] List<scr_Item> playerItems = new List <scr_Item>();
	[SerializeField] scr_AudioClient audioClient;
	UnityEvent itemChangeCallback;

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
					itemChangeCallback.Invoke ();
					return true;
				} else {
					//Exceeded Maximum quantity
					return false;
				}
			}
		}


		newItem.setPlayerReferences (this.gameObject);
		playerItems.Add (newItem);
		itemChangeCallback.Invoke ();
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

	public List<scr_Item> getAllItems(){
		return playerItems;
	}

	void Awake(){
		if (itemChangeCallback == null)
			itemChangeCallback = new UnityEvent();
	}
	void Update(){
		///TEST INPUTS / FUNCTIONALITIES
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if (playerItems [0].useItem ()) {
				audioClient.playAudioClip ("Repair", scr_AudioClient.sources.sfx);
				itemChangeCallback.Invoke ();
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if (playerItems [1].useItem ()) {
				audioClient.playAudioClip ("Refuel", scr_AudioClient.sources.sfx);
				itemChangeCallback.Invoke ();
			}
		}
	}
	private void OnDestroy()
	{
		if(itemChangeCallback != null)
			itemChangeCallback.RemoveAllListeners();
	}

	#region Callback method
	public void addItemChangeCallback(UnityAction call)
	{
		if(itemChangeCallback != null)
			itemChangeCallback.AddListener(call);
	}

	public void removeItemChangeCallback(UnityAction call)
	{
		if(itemChangeCallback != null)
			itemChangeCallback.RemoveListener(call);
	}

	#endregion
}
