using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scr trigger enter damage. Damages an gameobject with the script scr_HealthController
/// of a certain tag.
/// 
/// It's parameters may be set either in the Inspector or through the setParameters method.
/// </summary>
public class scr_TriggerDamage : MonoBehaviour {

	[Header("Trigger Callbacks")]
	[SerializeField] bool onEnter = true;
	[SerializeField] bool onStay;
	[SerializeField] bool onExit;

	[Header("Damage attributes")]
	[Tooltip("Knockback direction is Left or Right, depending on target velocity")]
	[SerializeField] bool repulseTarget;
	[SerializeField] string tagToDamage;
	[SerializeField] float damage;
	[SerializeField] float force;
	[SerializeField] Vector2 knockbackDirection;

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

	/// <summary>
	/// Damages the target. Used in the Trigger callbacks when the corresponding boolean is enabled
	/// </summary>
	/// <param name="col">Collider2D of the target</param>
	void damageTarget(Collider2D col){
		Rigidbody2D rb2d = col.GetComponent<Rigidbody2D>();
		scr_HealthController life = col.GetComponent<scr_HealthController> ();

		if (life != null) {

			if(!repulseTarget)
				life.takeDamage (damage, knockbackDirection * force);

			else{
				if(rb2d.velocity.x >= 0)
					life.takeDamage (damage, Vector2.right * force);;
				if(rb2d.velocity.x < 0)
					life.takeDamage (damage, Vector2.left * force);	
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (onEnter && col.CompareTag (tagToDamage))
			damageTarget (col);
	}
	void OnTriggerStay2D(Collider2D col){
		if (onEnter && col.CompareTag (tagToDamage))
			damageTarget (col);
	}
	void OnTriggerExit2D(Collider2D col){
		if (onEnter && col.CompareTag (tagToDamage))
			damageTarget (col);
	}
}
