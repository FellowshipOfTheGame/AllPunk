using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Hunt : FSM.State{
	
	[Header("Hunt Variables")]
	#region variables
	[SerializeField]
	scr_EnemyBoilerMaestro boilerMaestro;

	[SerializeField]
	float huntSpeed;
	[SerializeField]
	float smashRange;
	[SerializeField]
	float chargeRange;

	FSM.State nexState;

	#endregion

	void Awake(){
		if (stateMachine == null)
			stateMachine = GetComponent<FSM.StateMachine> ();
		if (boilerMaestro == null)
			boilerMaestro = GetComponent<scr_EnemyBoilerMaestro> ();
	}

	public override void Enter ()
	{
		print ("Entered Hunt");
	}

	public override void Execute ()
	{
		print ("Execute Hunt");

		if(boilerMaestro.Target == null){
			connectedStates.TryGetValue("Wander", out nexState);
			stateMachine.transitionToState (nexState);
		}

		boilerMaestro.faceTarget ();

		if (boilerMaestro.targetInRange (smashRange)) {
			connectedStates.TryGetValue("Smash", out nexState);
			stateMachine.transitionToState (nexState);
		}
		if (boilerMaestro.targetInRange (chargeRange)) {
			print ("CAN CHARGE");
		}

		if (!boilerMaestro.hasFloor () || boilerMaestro.hasObstacle ())
			boilerMaestro.horizontalMove (0f);
		else
			boilerMaestro.horizontalMove (huntSpeed);


	}

	public override void Exit ()
	{
	}


}
