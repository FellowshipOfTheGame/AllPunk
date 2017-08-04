using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_SteamBreath : scr_Weapon {

	public float meleeAtackDistance = 3.0f;
	public float knockbackIntensity = 2.0f;

	//private Transform spawnPosition;//Posição para spawnar hitbox

	private void Awake()
	{
		base.Awake();
		//spawnPosition = transform.Find("SpawnPosition");
	}


	override protected void AttackAction(bool noAnimation) {
	/**
	 * Para o steam breath, projetar uma Hitbox que realizará dano
	 * zero e dará knockback em um vetor na direção mirada
	 */
		if (noAnimation && clicked) {

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 weaponDirection = mouseWorldPosition - transform.position;

			Vector2 pos = new Vector2(transform.position.x + meleeAtackDistance / 2, transform.position.y);

			Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(meleeAtackDistance / 2,2), 0);
			print ("breath");
			foreach (Collider2D hit in hits) {

				scr_HealthController entity = hit.GetComponent<scr_HealthController> ();
				if (entity != null) {
					print (entity);
					entity.takeDamage (0, weaponDirection.normalized * knockbackIntensity);
				}

			}

			//StartAttackAnimation();
		}
	}

}
