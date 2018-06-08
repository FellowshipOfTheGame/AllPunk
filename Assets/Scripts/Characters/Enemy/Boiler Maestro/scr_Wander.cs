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
		///TODO: Check if Player is sighted, if so, transition to Hunt


		//Will fall down
		if (!boilerMaestro.hasFloor() || boilerMaestro.hasObstacle())
			boilerMaestro.Flip ();

		if(boilerMaestro.IsFacingRight)
			boilerMaestro.rb2D.velocity = new Vector2(1 * wanderSpeed, boilerMaestro.rb2D.velocity.y);
		else
			boilerMaestro.rb2D.velocity = new Vector2(-1 * wanderSpeed, boilerMaestro.rb2D.velocity.y);
		

		boilerMaestro.animator.SetBool("IsGrounded", boilerMaestro.isGrounded());
		boilerMaestro.animator.SetFloat("HorizontalSpeed", Mathf.Abs(boilerMaestro.rb2D.velocity.x));
		boilerMaestro.animator.SetFloat("VerticalSpeed", boilerMaestro.rb2D.velocity.y);
	}

	public override void Exit ()
	{
		throw new System.NotImplementedException ();
	}
		
}
