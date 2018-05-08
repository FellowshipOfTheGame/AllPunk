using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interface to be implemented by all Enchancement Parts that generate energy
/// The way the player's primary energy recharges and the amount of primary energy
/// are the ONLY resposabilities of Generator Enchancement Parts
/// </summary>
public abstract class scr_Rechargable:MonoBehaviour {

	private UnityEvent energyRechargeCallback;

	float maxEnergy;

	/// <summary>
	/// Gets the max energy of this Enchancement Part
	/// </summary>
	/// <returns>The max energy.</returns>
	public float getMaxEnergy(){
		return maxEnergy;
	}

	public void setEnergyRegenCallback(UnityAction call)
	{
		if(energyRechargeCallback != null)
			energyRechargeCallback.AddListener(call);
	}

	/// <summary>
	/// Sts the energy references to the player
	/// </summary>
	/// <param name="playerEnergy">Player energy.</param>
	public void setEnergyReferences (scr_PlayerEnergyController playerEnergy){
		playerEnergy.setMaxPrimEnergy (this.maxEnergy);
	}

	/// <summary>
	/// Recharges the primary energy.
	/// Depends on the Part
	/// </summary>
	public abstract void rechargePrimEnergy ();


	/*
	 * Updates or other codes are dependent of the Piece
	 */
}
