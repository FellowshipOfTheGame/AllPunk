using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Enemy : MonoBehaviour {

	public float Health;
	public bool ReceiveKnockback;
	private Rigidbody2D rb2D;

	void Awake() {
		rb2D = GetComponent<Rigidbody2D> ();
		
	}

	void Damage(float Damage, Vector3 HitPosition, float Force){
        Health -= Damage;

        if (Health <= 0)
        {
            Die();
            return;
        }

        if (ReceiveKnockback)
        {
            Vector3 direction = HitPosition - this.transform.position;
            rb2D.AddForce(direction.normalized * Force / rb2D.mass,ForceMode2D.Impulse);
        }
	}

	void Die(){
        Destroy(this);
	}

}
