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
	public float setUpTime;

	private float currTimeToFire;
	private float currSetUpTime;

	public GameObject projectilePrefab;
	//Child object do cano da arma
	private GameObject barrel;

	/**DESABILITADO PARA DEMO
	 * Game object vazio que armazena a posição do fim do laser. Usado para fazer a translação suave entre
	 *a posição do fim laser e a posição do alvo. 
	 */
	private GameObject aimFocus;

	//Direção do projétil
	private Vector3 direction;

	#endregion

	#region timer functions
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
	#endregion

	// Use this for initialization
	void Awake () {
		currSetUpTime = setUpTime;
		currTimeToFire = rateOfFire;

		lineRen = GetComponentInChildren<LineRenderer>();
		triggerZone = GetComponent<BoxCollider2D> ();

		barrel = this.gameObject.transform.GetChild (0).gameObject;
		//aimFocus = this.gameObject.transform.GetChild (0).GetChild(0).gameObject;	//TENTATIVA

		//Posição original do laser é a própria torreta----
		lineRen.SetPosition (0, barrel.transform.position);
		lineRen.SetPosition (1, barrel.transform.position);

		direction = Vector3.zero;
	}

	/**
	 * ALVO ENTROU NA KILLZONE
	 * Setar alvo, alterar posição do Line renderer
	 */
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			lineRen.SetPosition(1, barrel.transform.position);
			target = col.gameObject;
		}
	}

	/**
	 * ALVO SAIU DA KILLZONE
	 * Deixar alvo como nulo, resetar o line renderer e resetar timers
	 */
	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			target = null;

			lineRen.SetPosition(1, barrel.transform.position);
			lineRen.widthMultiplier = 1;

			resetTimer(ref currSetUpTime, setUpTime);
			resetTimer (ref currTimeToFire, rateOfFire);
		}
	}

	void Update () {

		//print ("S: " + currSetUpTime  + " F: " + currTimeToFire);

		//Torreta tem um alvo válido
		if (target != null) {

			/* COMENTADO PARA A DEMO --
			//Se a mira não está encima do alvo, mova a mira 
			/*if (aimFocus.transform.position != target.transform.position) {
				//print ("a: " + aimFocus.transform.position + " t: " + target.transform.position + " diff: " + Vector3.Normalize(target.transform.position - aimFocus.transform.position));
				//aimFocus.transform.Translate(Vector3.Normalize(-target.transform.position + aimFocus.transform.position) * 1 * Time.deltaTime);
				aimFocus.transform.position = target.transform.position;
			}*/
			//lineRen.SetPosition (1, aimFocus.transform.position); //COMENTADO PELA DEMO*/


			lineRen.SetPosition(1, target.transform.position);
			direction = lineRen.GetPosition (1) - lineRen.GetPosition (0);
			//direction = aimFocus.transform.position - this.gameObject.transform.position;

			//Altera a rotação do cano
			barrel.transform.rotation = Quaternion.LookRotation(direction.normalized);
			//Hack para que o sprite não saia do plano XY
			barrel.transform.right = direction;

			//Decrementa timer de setup
			if (currSetUpTime > 0) {
				decrementTimer (ref currSetUpTime);
				//lineRen.endColor = Color.green;
				lineRen.startColor = Color.green;
				lineRen.material.color = new Color(1f,1f,1f,0f);
			}

			//Setup finalizado , pronto para disparar
			if(currSetUpTime <= 0)
				lineRen.startColor = Color.red;

			//Setup feito mas projetil "recarregando"
			if (currTimeToFire > 0 && currSetUpTime <=0) {
				decrementTimer (ref currTimeToFire);
				lineRen.widthMultiplier = currTimeToFire / rateOfFire;
			}

			//Setup pronto, pode atirar
			if (currSetUpTime <= 0 && currTimeToFire <= 0) {

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
