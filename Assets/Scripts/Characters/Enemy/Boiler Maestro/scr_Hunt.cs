using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Hunt : FSM.State{
	
	[Header("Hunt Variables")]
	#region variables
	[SerializeField]
	scr_EnemyBoilerMaestro boilerMaestro;

	//[SerializeField]
	//float wanderSpeed;

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
			FSM.State wanderState;
			connectedStates.TryGetValue("Wander", out wanderState);
			stateMachine.transitionToState (wanderState);
		}

		boilerMaestro.faceTarget ();

		if (!boilerMaestro.hasFloor () || boilerMaestro.hasObstacle ())
			boilerMaestro.horizontalMove (0f);
		else
			boilerMaestro.horizontalMove (10f);


	}

	public override void Exit ()
	{
	}


}
