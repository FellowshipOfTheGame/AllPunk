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

	#endregion

	void Awake(){
		if (stateMachine == null)
			stateMachine = GetComponent<FSM.StateMachine> ();
		if (boilerMaestro == null)
			boilerMaestro = GetComponent<scr_EnemyBoilerMaestro> ();
	}

	public override void Enter ()
	{

	}

	public override void Execute ()
	{
		if(boilerMaestro.Target != null){
			FSM.State huntState;
			connectedStates.TryGetValue("Hunt", out huntState);
			stateMachine.transitionToState (huntState);
		}


		//ObstacleCheck
		if (boilerMaestro.isGrounded() && (!boilerMaestro.hasFloor() || boilerMaestro.hasObstacle()))
			boilerMaestro.Flip ();

		boilerMaestro.horizontalMove (wanderSpeed);


	}

	public override void Exit ()
	{
	}
		
}
