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

	int direction;

	#endregion

	void Awake(){
		if (stateMachine == null)
			stateMachine = GetComponent<FSM.StateMachine> ();
		if (boilerMaestro == null)
			boilerMaestro = GetComponent<scr_EnemyBoilerMaestro> ();
	}

	public override void Enter ()
	{
		direction = (boilerMaestro.IsFacingRight) ? 1 : -1;

	}

	public override void Execute ()
	{
		boilerMaestro.rb2D.velocity = new Vector2(direction * wanderSpeed, boilerMaestro.rb2D.velocity.y);
		boilerMaestro.animator.SetBool("IsGrounded", boilerMaestro.isGrounded());
		boilerMaestro.animator.SetFloat("HorizontalSpeed", Mathf.Abs(boilerMaestro.rb2D.velocity.x));
		boilerMaestro.animator.SetFloat("VerticalSpeed", boilerMaestro.rb2D.velocity.y);
	}

	public override void Exit ()
	{
		throw new System.NotImplementedException ();
	}
		
}
