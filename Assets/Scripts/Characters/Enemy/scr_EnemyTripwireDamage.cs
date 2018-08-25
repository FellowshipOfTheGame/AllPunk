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
[Tooltip("Tempo de cooldown entre danos")]
[SerializeField]
float cooldownTimer = 0.5f;

private Vector2 resultVector;

//O inimigo pode causar dano ou nao (ligado ao tempo de cooldown
private bool canCauseDamage = true;

private void OnCollisionStay2D(Collision2D targetRangeHit) {

if (canCauseDamage && targetRangeHit.collider.CompareTag ("Player")) {
scr_HealthController health = targetRangeHit.collider.gameObject.GetComponent<scr_HealthController>();
resultVector = targetRangeHit.transform.position - transform.position;
resultVector.Normalize();
if (health != null)
{
	//health.takeDamage(touchDamage, -1 * targetRangeHit.rigidbody.velocity.normalized * repulseForce);
	if(resultVector.x >= 0)
		health.takeDamage(touchDamage, Vector2.right * repulseForce);
	else
		health.takeDamage(touchDamage, Vector2.left * repulseForce);		
}
canCauseDamage = false;
StartCoroutine(waitCooldown());
}
}

private IEnumerator waitCooldown(){
float counter = 0;
while(counter < cooldownTimer){
counter += Time.deltaTime;
yield return null;
}
canCauseDamage = true;
}

}
