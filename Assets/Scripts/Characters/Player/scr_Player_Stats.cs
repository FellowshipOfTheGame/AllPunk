using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class scr_Player_Stats {

	[Header("Player properties")]
	//Maximo da vida
	public float maxHp;
	//Vida atual
	public float currentHp;
	//Defesa, usada para diminuir dano
	public float defense;
	public float poise;
	//Energy variables
	public float currentEnergy;
	public float maxEnergy;
	public float rechargeRate;

	[Header("Equipped parts")]
	public scr_PA_Manager.WeaponPart leftWeaponEquiped;
	public scr_PA_Manager.WeaponPart rightWeaponEquiped;
	public scr_PA_Manager.HeadPart headEquiped;
	public scr_PA_Manager.LegPart legsEquiped;
	public scr_PA_Manager.TorsoPart torsoEquiped;

	[Header("Unlocked parts")]
	[SerializeField]
	public List<scr_PA_Manager.WeaponPart> unlockedWeapons;
	[SerializeField]
	public List<scr_PA_Manager.LegPart> unlockedLegs;
	[SerializeField]
	public List<scr_PA_Manager.TorsoPart> unlockedTorsos;
	[SerializeField]
	public List<scr_PA_Manager.HeadPart> unlockedHeads;

	[Header("Save variables")]
	//Save variables
	/// <summary>
	/// Name of the scene where the player saved for the last time
	/// </summary>
	public string savePointScene = "null";
	/// <summary>
	/// Name of the object which the game had saved
	/// </summary>
	public string savePointName = "null";

	public scr_Player_Stats() {
		leftWeaponEquiped = scr_PA_Manager.WeaponPart.None;
		rightWeaponEquiped = scr_PA_Manager.WeaponPart.None;
		headEquiped = scr_PA_Manager.HeadPart.None;
		legsEquiped = scr_PA_Manager.LegPart.None;
		torsoEquiped = scr_PA_Manager.TorsoPart.None;

		unlockedWeapons = new List<scr_PA_Manager.WeaponPart>();
		unlockedWeapons.Add(scr_PA_Manager.WeaponPart.None);

		unlockedHeads = new List<scr_PA_Manager.HeadPart>();
		unlockedHeads.Add(scr_PA_Manager.HeadPart.None);

		unlockedLegs = new List<scr_PA_Manager.LegPart>();
		unlockedLegs.Add(scr_PA_Manager.LegPart.None);

		unlockedTorsos = new List<scr_PA_Manager.TorsoPart>();
		unlockedTorsos.Add(scr_PA_Manager.TorsoPart.None);
	}
}
