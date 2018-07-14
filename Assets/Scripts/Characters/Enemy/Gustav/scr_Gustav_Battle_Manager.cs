using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Battle_Manager : MonoBehaviour {

	public bool hasReachedStart = false;
	public bool hasReachedEnd = false;
	public GameObject bulletPrefab;
	public Vector3 bulletOffset;
	public GameObject batpipperPrefab;
	public Vector3 spawnOffset;
	public float lifePorcent;
	public scr_Gustav_Background_Scroller scroller;

	private FSM.StateMachine stateMachine;
	private List<scr_Gustav_Elevating_Plataform> plataforms;
	private GameObject player;


	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		plataforms = new List<scr_Gustav_Elevating_Plataform>();
	}

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void FixedUpdate(){
		stateMachine.UpdateState ();
	}


	public void addPlataform(scr_Gustav_Elevating_Plataform plat) {
		if(plataforms != null)
			plataforms.Add(plat);
	}

	public void removePlataform(scr_Gustav_Elevating_Plataform plat) {
		if(plataforms != null)
			plataforms.Remove(plat);
	}

	public void setReachedStart(bool hasReached) {
		hasReachedStart = hasReached;
	}

	public void setReachedEnd(bool hasReached) {
		hasReachedEnd = hasReached;
	}

	public void elevate(){
		foreach (scr_Gustav_Elevating_Plataform plat in plataforms) {
			if(plat != null)
				plat.elevate();
		}
	}

	public void descent(){
		foreach (scr_Gustav_Elevating_Plataform plat in plataforms) {
			if(plat != null)
				plat.descent();
		}
	}

	public void playSound() {
		Debug.Log("TOCA SOM");
	}

	public void shoot() {
		GameObject projectile = GameObject.Instantiate(bulletPrefab,player.transform.position+bulletOffset, Quaternion.identity);
		scr_GustavProjectile pScript = projectile.GetComponent<scr_GustavProjectile>();
		pScript.FireStraight(player.transform, "Player");
	}

}
