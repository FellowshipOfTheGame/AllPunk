using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class scr_Player_Stats {

	[Header("Propriedades de vida")]
	[Tooltip("Maximo da vida")]
	public float maxHp;
	[Tooltip("Vida atual")]
	public float currentHp;
	[Tooltip("Defesa, usada para diminuir dano")]
	public float defense;
	[Tooltip("Poise, determina o quanto o jogador pode ser empurrado")]
	public float poise;
	[Header("Propriedades de energia")]
	[Tooltip("Quantidade atual de energia reserva")]
	public float currentResEnergy;
	[Tooltip("Quantidade máxima de enegia reserva")]
	public float maxResEnergy;
	[Tooltip("Taxa de recarga de energoa")]
	public float rechargeRate;
	[Tooltip("Quantidade atual de energia primária")]
	//Energy variables - primary
	public float currentPrimEnergy;
	[Tooltip("Quantidade maxima de energia primária")]
	public float maxPrimEnergy;

	[Header("Propriedades de equipamento")]
	[SerializeField]
	public StringBoleanDictionary unlockedEPs;
	public string leftWeaponEquiped = "None";
	public string rightWeaponEquiped = "None";
	public string headEquiped = "None";
	public string legsEquiped = "None";
	public string torsoEquiped = "None";

	[Header("Pickups unicos")]
	public StringBoleanDictionary takenPickups;

	[Header("Cenas conhecidas")]
	public StringBoleanDictionary scenesDiscovered;

	[Header("Propriedades de save")]
	[Tooltip("Nome da cena onde o jogador salvou pela última vez")]
	public string savePointScene = "null";
	[Tooltip("Nome do objecto onde foi salvo o jogo")]
	public string savePointName = "null";

	public scr_Player_Stats() {
		unlockedEPs = new StringBoleanDictionary();
		scenesDiscovered = new StringBoleanDictionary();
	}
}
