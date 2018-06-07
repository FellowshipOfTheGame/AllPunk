﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EP_Boiler : scr_EP {


	[Header("Boiler Variables")]
	/// <summary>
	/// Reference to The player energy controller.
	/// </summary>
	[SerializeField] scr_PlayerEnergyController playerEnergy;

	/// <summary>
	/// Amount of Energy of the Boiler EP
	/// </summary>
	[SerializeField] float boilerEnergy;

	public override bool Equip (GameObject playerReference)
	{
		playerEnergy = playerReference.GetComponent<scr_PlayerEnergyController>();
		if (playerEnergy != null) {
			playerEnergy.setMaxPrimEnergy (boilerEnergy);
			return true;
		} else
			return false;
	}

	public override bool Unequip ()
	{
		if (playerEnergy != null) {
			playerEnergy.setMaxPrimEnergy (0);
			return true;
		} else
			return false;
	}

	/// <summary>
	/// Increases the player primary Energy. Called by scr_Item_Coal when it is used.
	/// </summary>
	/// <returns><c>true</c>, if coal was burned, <c>false</c> otherwise.</returns>
	/// <param name="energyInc">Energy increment</param>
	public bool burnCoal(float energyInc){
		if (playerEnergy.currPrimEnergy < boilerEnergy) {
			playerEnergy.currPrimEnergy = Mathf.Clamp (playerEnergy.currPrimEnergy + energyInc, 0, boilerEnergy);
			return true;
		} else
			return false;
	}
}