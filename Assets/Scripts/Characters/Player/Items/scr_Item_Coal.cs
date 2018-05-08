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
		if (playerBoiler.burnCoal (enInc) && currQty > 0){
			currQty--;
			return true;
		}
		else
			return false;
	}


	public void setPlayerReferences(GameObject playerObject){
		/*if(playerObject.EPManager.currTorso=="boiler")
			playerBoiler = UnlockedEps.get("boiler");
		*/
		//playerBoiler = playerObject.GetComponent<scr_EP_Boiler> ();

		//PARA TESTAR - COLOQUE UM BOILER NO PREFAB DO SIGMA E ARRASTE PARA A REFERENCIA DO COAL
		throw new System.NotImplementedException ();
	}

	#endregion


}
