﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyOniTurret : MonoBehaviour {

	#region variables
	private LineRenderer lineRen;
	private BoxCollider2D triggerZone;
	private GameObject target;

	//Tempo em segundos entre tiros
	public float rateOfFire;
	//Tempo em segundos que leva para começar a atacar
	public float setUpTime;

	private float currTimeToFire;
	private float currSetUpTime;

	public GameObject projectilePrefab;
	//Child object do cano da arma
	private GameObject barrel;
	#endregion
	/***
	 * Função genérica para decrementar um timer
	 * @timer = referencia (ponteiro) para o timer a ser decrementado
	 */
	void decrementTimer(ref float timer){
		if (timer > 0)
			timer -= Time.deltaTime;
		else if (timer <= 0)
			timer = 0;
	}
		
	void resetTimer(ref float timer, float timeToReset){
		timer = timeToReset;
	}

	// Use this for initialization
	void Awake () {
		currSetUpTime = setUpTime;
		currTimeToFire = 0;//Se ela fez o setup, estará pronta para atirar

		lineRen = GetComponent<LineRenderer> ();
		triggerZone = GetComponent<BoxCollider2D> ();

		barrel = this.gameObject.transform.GetChild (0).gameObject;

		//Posição original do laser é a própria torreta
		lineRen.SetPosition (0, barrel.transform.position);
		lineRen.SetPosition (1, barrel.transform.position);
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			target = col.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			target = null;
			lineRen.SetPosition(1, barrel.transform.position);
			//currSetUpTime = setUpTime;
			resetTimer(ref currSetUpTime, setUpTime);
		}
	}

	// Update is called once per frame
	void Update () {
		
		if (currTimeToFire != 0) {
			decrementTimer (ref currTimeToFire);
			lineRen.widthMultiplier = currTimeToFire / rateOfFire;
		}
		
		if (target != null) {
			lineRen.SetPosition (1, target.transform.position);

			Vector3 direction = lineRen.GetPosition (1) - lineRen.GetPosition (0);


			//Altera a rotação do cano
			barrel.transform.rotation = Quaternion.LookRotation(direction.normalized);
			//Hack para que o sprite não saia do plano XY
			barrel.transform.right = direction;

			//Decrementa timer de setup
			if (currSetUpTime != 0) {
				decrementTimer (ref currSetUpTime);
				//lineRen.endColor = Color.green;
				lineRen.startColor = Color.green;
				lineRen.material.color = new Color(1f,1f,1f,0.5f);
			}

			//Setup pronto, pode atirar
			if (currSetUpTime == 0 && currTimeToFire == 0) {
				//lineRen.endColor = Color.red;
				lineRen.startColor = Color.red;
				
				GameObject projectile = GameObject.Instantiate (projectilePrefab,
								barrel.transform.position+direction.normalized*3,
								barrel.transform.rotation);

				scr_Projectile projectileScr = projectile.GetComponent<scr_Projectile> ();

				projectileScr.Fire (direction, this.tag);

				//reseta tempo para atirar
				resetTimer(ref currTimeToFire, rateOfFire);
			}
		}
	}
}