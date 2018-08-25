using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Charge : FSM.State {

	[Header("Charge variables")]
	#region variables
	[SerializeField]
	scr_EnemyBoilerMaestro boilerMaestro;

	[SerializeField]
	float chargeSpeed;
	[SerializeField]
	float pushForce;
	[SerializeField]
	float pushDamage;

	//Find with started running
	bool isRunning = false;
	bool hasHit = false;
	bool hasFinishedRunning = false;

	scr_HealthController healthCont;
	float initialPoise;

	[SerializeField]
	scr_AudioClient audioClient;
	#endregion

	public override void Enter ()
	{
		Debug.Log("Enterred charge");
		boilerMaestro.animator.SetBool("Charge", true);
		isRunning = false;
		hasHit = false;
		hasFinishedRunning = false;

		healthCont = boilerMaestro.GetComponent<scr_HealthController>();
		initialPoise = healthCont.poise;
		healthCont.poise = 1;
		
	}

	public override void Execute ()
	{
		if(!isRunning) {
			if(!hasFinishedRunning && boilerMaestro.animator.GetCurrentAnimatorStateInfo(0).IsName("Running")){
				audioClient.playAudioClip ("Charge", scr_AudioClient.sources.local);
				isRunning = true;
				boilerMaestro.playParticle();
			}
			if(hasFinishedRunning && boilerMaestro.animator.GetCurrentAnimatorStateInfo(0).IsName("Move")){
				FSM.State huntState;
				connectedStates.TryGetValue("Hunt", out huntState);
				stateMachine.transitionToState(huntState);
			}
			boilerMaestro.horizontalMove(0);
			
		}

		if(isRunning) {
			if(!boilerMaestro.hasFloor() || boilerMaestro.hasObstacle()){
				boilerMaestro.animator.SetTrigger("FinishCharge");
				boilerMaestro.animator.SetBool("Charge", false);
				isRunning = false;
				healthCont.poise = initialPoise;
				hasFinishedRunning = true;
				boilerMaestro.horizontalMove(0);
			}
			else{
				boilerMaestro.horizontalMove(chargeSpeed);
			}

			if(!hasHit && boilerMaestro.isColidingWithPlayer()){
				GameObject player = boilerMaestro.playerCollisionReference();
				if(player != null){
					Vector2 direction = ((player.transform.position - boilerMaestro.transform.position).x > 0) ? Vector2.right : Vector2.left;
					scr_HealthController pHealth = player.GetComponent<scr_HealthController>();
					pHealth.takeDamage(pushDamage, direction * pushForce);
					hasHit = true;
				}
			}
		}
	}

	public override void Exit ()
	{
		Debug.Log("Exited charge");
		
	}
}
