using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_DeathPit : MonoBehaviour {

	public float damage;

	public void OnCollisionEnter2D(Collision2D col){
		scr_HealthController entity = col.gameObject.GetComponent<scr_HealthController> ();
		entity.takeDamage (damage, Vector2.zero);
	}
}
