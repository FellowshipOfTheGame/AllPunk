using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Item_RepairKit : MonoBehaviour, scr_Item {

	#region variables
	[SerializeField] int currQty;
	[SerializeField] int maxQty;
	[SerializeField] int hpInc;

	public scr_HealthController playerHealth;
	#endregion

	#region Item Implementations
	public int getCurrQty (){
		return currQty;
	}


	public void setCurrQty(int newQty){
		currQty = Mathf.Clamp (newQty, 0, maxQty);
	}


	public int getMaxQty (){
		return maxQty;
	}

	public bool useItem (){
		if (playerHealth!=null && currQty > 0 && playerHealth.getCurrentHealth() < playerHealth.getMaxHealth()) {
			playerHealth.removeDamage (hpInc);
			currQty--;
			return true;
		}
		else
			return false;
	}


	public void setPlayerReferences(GameObject playerObject){
		playerHealth = playerObject.GetComponent<scr_HealthController> ();
		//print ("PLAYA" + playerObject);
	}

	#endregion


}
