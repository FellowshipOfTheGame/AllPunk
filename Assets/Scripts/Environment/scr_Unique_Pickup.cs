﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Unique_Pickup : MonoBehaviour {

	[Tooltip("Deve destruir o objeto se houver sido pego anteriormente")]
	public bool shouldDestroyIfExists = true;
	[Tooltip("String única pra identificar esse pickup")]
	public string uniqueKey;

	public string checkEP = "";

	// Use this for initialization
	void Start () {
		//Verifica se já existe
		scr_Player_Stats player_Stats = scr_GameManager.instance.playerStats;
		if(player_Stats.takenPickups.ContainsKey(uniqueKey)){
			if(player_Stats.takenPickups[uniqueKey] == true && shouldDestroyIfExists){
				//Check if player has the EP before destroying
				scr_EPManager ePManager = 
				(scr_GameManager.instance.player != null) ? scr_GameManager.instance.player.GetComponent<scr_EPManager>() : null;
				if(string.IsNullOrEmpty(checkEP) || ePManager == null || !ePManager.UnlockedEPs.ContainsKey(checkEP) || ePManager.UnlockedEPs[checkEP])
					Destroy(gameObject);
			}
		}
		else{
			scr_GameManager.instance.playerStats.takenPickups.Add(uniqueKey,false);
		}
	}
	
	private void OnDestroy() {
		scr_GameManager.instance.playerStats.takenPickups[uniqueKey] = true;
	}
}
