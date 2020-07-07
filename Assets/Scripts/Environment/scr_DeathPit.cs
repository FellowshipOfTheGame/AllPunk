using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_DeathPit : MonoBehaviour {

	public float damage;
    public enum triggerType {
        trigger,contact
    };
    public triggerType triggerMode = triggerType.trigger;

    public void OnTriggerEnter2D (Collider2D collision)
    {
        if(triggerMode == triggerType.trigger) {
            if(collision.gameObject.tag == "Player")
                scr_GameManager.instance.startGameOver();
            
            scr_HealthController entity = collision.gameObject.GetComponent<scr_HealthController>();
            if(entity)
                entity.takeDamage(damage, Vector2.zero);
            
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if(triggerMode == triggerType.contact) {
            if(collision.gameObject.tag == "Player")
                scr_GameManager.instance.startGameOver();
            scr_HealthController entity = collision.gameObject.GetComponent<scr_HealthController>();
            entity.takeDamage(damage, Vector2.zero);
        }
    }

}
