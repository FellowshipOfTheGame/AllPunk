﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_SteamBreath : scr_Weapon {

	public float meleeAtackDistance = 10.0f;
	public float knockbackIntensity = 200.0f;

	public float overlapBoxWidth = 4.0f;
	public float overlapBoxHeight = 4.0f;

	public GameObject pointPrefab;
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
			Vector2 weaponDirection = mouseWorldPosition - transform.position; 

			weaponDirection = weaponDirection.normalized;

			Vector2 pos = mouseWorldPosition;

			//Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(meleeAtackDistance / 2, 4), 0);
			Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(overlapBoxWidth , overlapBoxWidth), 0);

			GameObject ponto = GameObject.Instantiate(pointPrefab, pos, transform.rotation);

			foreach (Collider2D hit in hits) {

				scr_HealthController entity = hit.GetComponent<scr_HealthController> ();
				if (entity != null) {
					print (entity);
					//entity.takeDamage (0, weaponDirection.normalized * knockbackIntensity);new Vector2 (weaponDirection.x, weaponDirection.y)
					print("VecWDir " + weaponDirection);
					print("VecKnock " + weaponDirection * knockbackIntensity);
					entity.takeDamage (0, weaponDirection * knockbackIntensity);

				}

			}

			//StartAttackAnimation();
		}
	}

}
