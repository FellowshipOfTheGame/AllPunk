using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_SteamBreath : scr_Weapon {

	#region variables

	public float meleeAtackDistance = 1.0f;
	public float knockbackIntensity;
	public float timeToFire = 1.0f;
	public GameObject particlePlayer; //Filho que deve ter o ParticleSystem

	//private Transform spawnPosition;//Posição para spawnar hitbox
	/*private float currentTimeToFire;*/

	#endregion variables

	public void Awake()
	{
		base.Awake();
		//currentTimeToFire = 0;
	}
		

	void OnDrawGizmos(){
		Gizmos.DrawLine (Camera.main.ScreenToWorldPoint (Input.mousePosition), lowerArm.position);
	}

	override protected void AttackAction(bool noAnimation) {
	/**
	 * Para o steam breath, projetar uma Hitbox que realizará dano
	 * zero e dará knockback em um vetor na direção mirada
	 */
		if (clicked || holding) {

            useEnergy();

			//spawnPosition = particlePlayer.transform;

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 weaponDirection = mouseWorldPosition - lowerArm.position; 

			/*Vector2 pos = new Vector2 (spawnPosition.position.x + weaponDirection.normalized.x,
				              spawnPosition.position.y + weaponDirection.normalized.y);*/

			Collider2D[] hits = new Collider2D[10];
			PolygonCollider2D collider = GetComponent<PolygonCollider2D>();//Referencia para o collider 
			ContactFilter2D ct2D = new ContactFilter2D();
			collider.OverlapCollider(ct2D, hits);

			//Instancia particleplayer
			if (particlePlayer != null) {
				GameObject o = GameObject.Instantiate (particlePlayer, transform.position, Quaternion.FromToRotation(Vector2.right, weaponDirection), null);
				o.GetComponent<ParticleSystem>().Play();
				GameObject.Destroy (o, 3.0f);
			}

			foreach (Collider2D hit in hits) {
				if (hit == null)
						continue;
			
				scr_HealthController entity = hit.GetComponent<scr_HealthController> ();
				if (entity != null && entity.tag != "Player") {
					print (entity);
					entity.takeDamage (0, weaponDirection.normalized * knockbackIntensity);
				}
			}
			//StartAttackAnimation();
		}
	}
}
