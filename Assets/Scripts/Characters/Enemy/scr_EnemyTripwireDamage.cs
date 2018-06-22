using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// tripwire damage.
/// If player enters the Tripwire, damages the player
/// Used if player is above the enemy
/// </summary>
public class scr_EnemyTripwireDamage : MonoBehaviour {

    [Tooltip("Dano recebido pelo jogador ao tocar no inimigo")]
	[SerializeField] float touchDamage = 10f;
    [Tooltip("Forca recebida pelo jogador ao tocar no inimigo")]
	[SerializeField] float repulseForce = 10f;

	private Vector2 resultVector;
	private RaycastHit2D targetRangeHit;

    //O inimigo pode causar dano ou nao (ligado ao tempo de cooldown
    private bool canCauseDamage = true;

	void Start(){
	}

	void OnTriggerStay2D(){

		if (targetRangeHit.collider != null && targetRangeHit.collider.CompareTag ("Player")) {
			scr_HealthController health = targetRangeHit.collider.gameObject.GetComponent<scr_HealthController>();
			if (health != null)
			{
				//health.takeDamage(touchDamage, -1 * targetRangeHit.rigidbody.velocity.normalized * repulseForce);
				if(targetRangeHit.rigidbody.velocity.x >= 0)
					health.takeDamage(touchDamage, Vector2.right * repulseForce);
				if(targetRangeHit.rigidbody.velocity.x < 0)
					health.takeDamage(touchDamage, Vector2.left * repulseForce);		
			}
		}
	}

		
}
