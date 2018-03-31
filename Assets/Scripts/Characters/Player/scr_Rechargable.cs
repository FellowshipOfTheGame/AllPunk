using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface to be implemented by all Enchancement Parts that generate energy
/// The way the player's primary energy recharges and the amount of primary energy
/// are the ONLY resposabilities of Generator Enchancement Parts
/// </summary>
public interface scr_Rechargable {

	/// <summary>
	/// Gets the max energy of this Enchancement Part
	/// </summary>
	/// <returns>The max energy.</returns>
	float getMaxEnergy();

	/// <summary>
	/// Sts the energy references to the player
	/// </summary>
	/// <param name="playerEnergy">Player energy.</param>
	void setEnergyReferences (scr_PlayerEnergyController playerEnergy);
}
