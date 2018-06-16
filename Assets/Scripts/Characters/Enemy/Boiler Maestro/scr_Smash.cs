using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Smash : FSM.State {
	
	[Header("Smash Variables")]
	#region variables
	[SerializeField]
	scr_EnemyBoilerMaestro boilerMaestro;

	[SerializeField]
	float smashDamage;
	[SerializeField]
	float smashForce;

	[SerializeField]
	Collider2D[] attackColliders;

	FSM.State nextState;

	#endregion

	public override void Enter ()
	{
		boilerMaestro.animator.SetTrigger ("Attack");
	}

	public override void Execute ()
	{
		bool correctAnimation = boilerMaestro.animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
		if (!correctAnimation)
		{
			connectedStates.TryGetValue ("Hunt", out nextState);
			stateMachine.transitionToState(nextState);
		}


		/*foreach (Collider2D hitCollider in attackColliders)
		{
			if (!hitCollider.isActiveAndEnabled)
				continue;
			Collider2D[] colHits = new Collider2D[10];
			ContactFilter2D ct2D = new ContactFilter2D();
			hitCollider.OverlapCollider(ct2D, colHits);
			foreach (Collider2D col in colHits)
			{
				if (col == null)
					break;

				scr_HealthController life = col.GetComponent<scr_HealthController>();
				if (life != null && col.tag != "Enemy")
				{
					Vector2 attackDir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
					life.takeDamage(smashDamage, attackDir*smashForce);
				}
			}
		}*/

		foreach (Collider2D hitCollider in attackColliders)
		{
			if (!hitCollider.isActiveAndEnabled)
				continue;
			Collider2D col = Physics2D.OverlapCircle (hitCollider.transform.position, 1f, LayerMask.GetMask ("Player"));

			if (col != null && col.CompareTag ("Player")) {
				scr_HealthController life = col.GetComponent<scr_HealthController>();
				Vector2 attackDir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
				life.takeDamage(smashDamage, attackDir*smashForce);
			}
		}
	}

	public override void Exit ()
	{
		
	}
}
