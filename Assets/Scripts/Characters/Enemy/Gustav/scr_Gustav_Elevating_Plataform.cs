using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Elevating_Plataform : MonoBehaviour {

	//Plataform sound
	[Tooltip("Pontos que vão ser spawnados inimigos")]
	public Transform[] spawnPoints;
	[Tooltip("Prefab do inimigo ou objeto a ser spawnado")]
	public GameObject enemyPrefab;
	[Tooltip("Deve spawnar entidades quando levanta")]
	public bool shouldSpawn = false;

	public bool DEBUG_GO_UP = false;
	public bool DEBUG_GO_DOWN = false;

	private List<GameObject> spawnedEnemies;
	private bool isElevated = false;
	private Animator animator;

	private void Awake() {
		animator = GetComponent<Animator>();
		spawnedEnemies = new List<GameObject>();
		if(shouldSpawn)
			animator.SetBool("Spawn", true);
		else
			animator.SetBool("Spawn", false);
	}

	/// <summary>
	/// Make the elevator go up
	/// </summary>
	public void elevate() {
		if(!isElevated)
			animator.SetTrigger("GoUp");
		isElevated = true;
	}

	/// <summary>
	/// Make the elevator go down
	/// </summary>
	public void descent() {
		if(isElevated)
			animator.SetTrigger("GoDown");
		isElevated = false;
	}

	/// <summary>
	/// Spawn enemies at the spawn points
	/// </summary>
	public void spawnEnemy() {
		for(int i = 0; i < spawnPoints.Length; i++) {
			GameObject spawn = GameObject.Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
			//spawnedEnemies.Add(spawn);
		}
	}

	/// <summary>
	/// Set wathever the elevator will spawn enemies when go up
	/// </summary>
	/// <param name="should"></param>
	public void setShouldSpawn(bool should) {
		shouldSpawn = should;
		if(shouldSpawn)
			animator.SetBool("Spawn", true);
		else
			animator.SetBool("Spawn", false);
	}

	/// <summary>
	/// Return if the elevator is up
	/// </summary>
	/// <returns>Is up</returns>
	public bool isElevatorElevated(){
		return isElevated;
	}

	//DEBUG ONLY
	private void Update() {
		if(DEBUG_GO_UP){
			elevate();
			DEBUG_GO_UP = false;
		}
		if(DEBUG_GO_DOWN) {
			descent();
			DEBUG_GO_DOWN = false;
		}
		
	}

}
