using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Item_Coal : MonoBehaviour, scr_Item {

	#region variables
	[SerializeField] int currQty;
	[SerializeField] int maxQty;
	[SerializeField] int enInc;

	public scr_EP_Boiler playerBoiler;
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
		///can only use the item If player has boiler equipped and has energy to increased

		if (playerBoiler == null)
			print ("PAU no PlayerBoiler");

		if (playerBoiler != null && playerBoiler.burnCoal (enInc) && currQty > 0){
			currQty--;
			return true;

		}
		else
			return false;
	}


	public void setPlayerReferences(GameObject playerObject){
		scr_EPManager epMan = playerObject.GetComponent<scr_EPManager>();

		/*print (playerObject);
		print (">> " + epMan.getCurrentPart (scr_EP.EpType.Torso));
		print (">| " + scr_EPManager.EPKeys.KEY_BOILER);*/


		if (epMan.getCurrentPart (scr_EP.EpType.Torso) == scr_EPManager.EPKeys.KEY_BOILER) {
			playerBoiler = (scr_EP_Boiler)epMan.getCurrentPartRef (scr_EP.EpType.Torso);
		}
		else
			playerBoiler = null;
	}

	#endregion


}
