using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_DeathPit : MonoBehaviour {

	public float damage;

    public void OnCollisionStay2D(Collision2D collision)
    {
        scr_HealthController entity = collision.gameObject.GetComponent<scr_HealthController>();
        entity.takeDamage(damage, Vector2.zero);
    }

}
