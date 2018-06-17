using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scr trigger enter damage. Damages an gameobject with the script scr_HealthController
/// of a certain tag
/// </summary>
public class scr_TriggerEnterDamage : MonoBehaviour {

	string tagToDamage;
	float damage;
	float force;
	Vector2 knockbackDirection;

	/// <summary>
	/// Sets the parameters.
	/// </summary>
	/// <param name="tag">Tag.</param>
	/// <param name="damage">Damage.</param>
	/// <param name="force">Force.</param>
	/// <param name="knockbackDirection">Knockback direction. Is normalized by this method</param>
	public void setParameters(string tag, float damage, float force, Vector2 knockbackDirection){
		this.tagToDamage = tag;
		this.damage = damage;
		this.force = force;
		this.knockbackDirection = knockbackDirection.normalized;
	}

	void OnTriggerEnter2D(Collider2D col){
		if (tagToDamage != "" && tagToDamage!=null) {
			if (col.CompareTag (tagToDamage)) {
				scr_HealthController life = col.GetComponent<scr_HealthController> ();
				if (life != null) {
					life.takeDamage (damage, knockbackDirection * force);
				}
			}
		}
	}
}
