using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerCombatController : MonoBehaviour {

	#region variables

	//Dano do ataque melee

	public float meleeAtackDamage;

	//Raio de distância entre a posição do player e o tiro

	[Range(2,4)]

	public float rangedAttackOffset = 3.0f;

	//Referencia ao projétil para ataque ranged.

	public GameObject projectile;

	public float meleeAtackDistance;

	#endregion variables



	#region MonoBehaviour methods

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		/*

		 * OBSERVAÇÃO SOBRE O ATAQUE:

		 * Futuramente seria interessante os botoes de ataque invocarem metodos dos scripts das

		 * P.A.s que ativam seus efeitos. Para principios de prototipacao, os efeitos ficarao

		 * no controlador do player

		 */

		if (Input.GetButtonDown("Fire1"))

			meleeAttack ();

		if (Input.GetButtonDown("Fire2")) 

			rangedAttack();
	}



	#endregion MonoBehaviour methods



	#region Combat methods


	void meleeAttack(){

		//Posiçao do centro do collider melee a ser usado

		Vector2 pos = new Vector2(transform.position.x + meleeAtackDistance / 2, transform.position.y);



		Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(meleeAtackDistance / 2,2), 0);



		foreach (Collider2D hit in hits) {

			if (hit.gameObject.tag == "Enemy") {

				scr_HealthController enemy = hit.GetComponent<scr_HealthController>();

				enemy.takeDamage(meleeAtackDamage, new Vector2(10, 0));

			}

		}

	}



	void rangedAttack(){

		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		mouseWorldPosition.z = transform.position.z;



		Vector3 gunDirection = mouseWorldPosition - transform.position;



		Vector3 gunPosition = gunDirection.normalized * rangedAttackOffset + transform.position;



		GameObject projectileClone = Instantiate (projectile, gunPosition, transform.rotation) as GameObject;

		scr_Projectile projScr = projectileClone.GetComponent<scr_Projectile>();

		projScr.Fire (gunDirection, this.tag);

	}

	#endregion Combat methods

}
