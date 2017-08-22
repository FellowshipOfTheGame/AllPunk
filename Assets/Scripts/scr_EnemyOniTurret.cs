using System.Collections;
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
	public float timeToFire;

	private float currRateOfFire;
	private float currTimeToFire;

	public GameObject projectilePrefab;

	#endregion

	// Use this for initialization
	void Awake () {
		currTimeToFire = timeToFire;
		currRateOfFire = rateOfFire;


		lineRen = GetComponent<LineRenderer> ();
		triggerZone = GetComponent<BoxCollider2D> ();


		//Posição original do laser é a própria torreta
		lineRen.SetPosition (0, gameObject.transform.position);
		lineRen.SetPosition (1, gameObject.transform.position);
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			target = col.gameObject;
		}
	}

	void OnTriggerStay2D(Collider2D col){
		/*Decrementa timer para atirar*/
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			target = null;
			lineRen.SetPosition(1, gameObject.transform.position);
		}
	}

	// Update is called once per frame
	void Update () {
		if (target != null) {
			lineRen.SetPosition (1, target.transform.position);
			GameObject projectile = GameObject.Instantiate(projectilePrefab,
				this.gameObject.transform.position,
				this.gameObject.transform.rotation);

			scr_Projectile projectileScr = projectile.GetComponent<scr_Projectile>();

			Vector3 direction = lineRen.GetPosition (1) - lineRen.GetPosition (0);
			projectileScr.Fire(direction, this.tag);
		}
	}
}
