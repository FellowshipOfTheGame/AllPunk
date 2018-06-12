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
	Collider2D[] attackColliders;

	FSM.State nexState;

	#endregion

	public override void Enter ()
	{
		//Toca a animaç'ão
	}

	public override void Execute ()
	{
		
	}

	public override void Exit ()
	{
		
	}
}
