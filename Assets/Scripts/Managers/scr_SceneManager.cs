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
	
	[Header("Musica")]
	//Musica principal
	[SerializeField]
	public scr_AudioClipWrapper musicClip;
	public float musicTransitionTime = 1f;

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
}
