using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Wander : FSM.State {

	[Header("Wander Variables")]
	#region variables
	[SerializeField]
	scr_EnemyBoilerMaestro boilerMaestro;

	[SerializeField]
	float wanderSpeed;
	
	[SerializeField]
	float wanderJump;

	bool jumping;

	#endregion

	void Awake(){
		if (stateMachine == null)
			stateMachine = GetComponent<FSM.StateMachine> ();
		if (boilerMaestro == null)
			boilerMaestro = GetComponent<scr_EnemyBoilerMaestro> ();
	}

	public override void Enter ()
	{
		jumping = false;
	}

	public override void Execute ()
	{
		if(boilerMaestro.Target != null){
			FSM.State huntState;
			connectedStates.TryGetValue("Hunt", out huntState);
			stateMachine.transitionToState (huntState);
		}

		bool grounded = boilerMaestro.isGrounded();
		if(grounded) jumping = false;
		bool hasStep = boilerMaestro.hasStep();

		//ObstacleCheck
		if (grounded && (!boilerMaestro.hasFloor() || (!hasStep && boilerMaestro.hasObstacle())))
			boilerMaestro.Flip ();

		if(grounded && hasStep && !jumping)
			jumping = boilerMaestro.jump(wanderJump, wanderSpeed);
		else if (!grounded && jumping)
			boilerMaestro.jump(wanderJump,wanderSpeed);
		else if (grounded)
			boilerMaestro.horizontalMove (wanderSpeed);


	}

	public override void Exit ()
	{
	}
		
}
