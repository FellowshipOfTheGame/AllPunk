using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_SceneManager : MonoBehaviour {

	/// <summary>
	/// Nome das cenas que se conectam à essa
	/// </summary>
	public string[] neighboorScenesReceive;
	/// <summary>
	/// Posição onde cada uma das cenas vizinhas levam nessa cena
	/// </summary>
	public Transform[] neighboorScenesDestination;
	public GameObject playerPrefab;

	public Transform positionToSpawnInScene(string originScene, scr_Player_Stats playerStats) {
		Transform result = null;
		if(!originScene.Equals("Load")) {
			//Procura pelo indice da cena de origem
			int findIndex = -1;
			for(int i=0; i < neighboorScenesReceive.Length; i++) {
				print("Found scene: " + neighboorScenesReceive[i]);
				if(neighboorScenesReceive[i].Equals(originScene)){
					findIndex = i;
					break;
				}
			}
			if(findIndex == -1) {
				print("Can't find origin scene in the new Scene. SceneName: " + originScene);
				return result;
			}

			result = neighboorScenesDestination[findIndex];
		}
		//Caso esteja carregando o nivel
		else {
			if(playerStats.savePointName.Equals("null")) {
				print("Save point name is null");
				return result;
			}
			GameObject spawnPoint = GameObject.Find(playerStats.savePointName);
			if(spawnPoint == null) {
				print("Can't find save object. Name: " + playerStats.savePointName);
				return result;
			}
			result = spawnPoint.transform;
		}
		return result;
	}

	public GameObject spawnPlayerFromScene(string originScene, scr_Player_Stats playerStats) {
		GameObject player = null;
		if(!originScene.Equals("Load")) {
			//Procura pelo indice da cena de origem
			int findIndex = -1;
			for(int i=0; i < neighboorScenesReceive.Length; i++) {
				print("Found scene: " + neighboorScenesReceive[i]);
				if(neighboorScenesReceive[i].Equals(originScene)){
					findIndex = i;
					break;
				}
			}
			if(findIndex == -1) {
				print("Can't find origin scene in the new Scene. SceneName: " + originScene);
				return player;
			}

			player = spawnPlayerOnTransform(playerStats, neighboorScenesDestination[findIndex]);
		}
		//Caso esteja carregando o nivel
		else {
			if(playerStats.savePointName.Equals("null")) {
				print("Save point name is null");
				return player;
			}
			GameObject spawnPoint = GameObject.Find(playerStats.savePointName);
			if(spawnPoint == null) {
				print("Can't find save object. Name: " + playerStats.savePointName);
				return player;
			}
			player = spawnPlayerOnTransform(playerStats, spawnPoint.transform);
		}
		return player;
	}

	private GameObject spawnPlayerOnTransform(scr_Player_Stats playerStats, Transform point) {
		GameObject player = GameObject.Instantiate(playerPrefab, point.position, Quaternion.identity);
		
		//Atualizar stats de vida
		scr_HealthController health = player.GetComponent<scr_HealthController>();
		health.maxHp = playerStats.maxHp;
		health.setCurrentHealth(playerStats.currentHp);
		health.defense = playerStats.defense;
		health.poise = playerStats.poise;

		//Atualizar stats de energia
		scr_PlayerEnergyController energy = player.GetComponent<scr_PlayerEnergyController>();
		energy.setMaxResEnergy(playerStats.maxResEnergy);
		energy.setCurrentResEnergy(playerStats.currentResEnergy);
		energy.setResRechargeRate (playerStats.rechargeRate);

		return player;
	}
}
