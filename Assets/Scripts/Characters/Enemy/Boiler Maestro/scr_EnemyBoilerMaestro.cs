using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyBoilerMaestro : MonoBehaviour {

	#region variables

	[SerializeField] FSM.StateMachine stateMachine;

	//Referência para rigibody 2D
	public Rigidbody2D rb2D;
	//Referencia do animator
	public Animator animator;

	//Saber se está indo para a direita
	private bool isFacingRight = true;


	#endregion


	void Awake(){
		if (stateMachine == null)
			stateMachine = GetComponent<FSM.StateMachine> ();
		if (animator == null)
			animator = GetComponent<Animator> ();
		if(rb2D == null)
			rb2D = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate(){
		stateMachine.UpdateState ();
	}


	/*
	public bool hasFloor(){
		
	}

	public bool hasWall(){
	
	}*/

	public bool isGrounded(){
		return true;
	}

	public bool IsFacingRight {
		get {
			return this.isFacingRight;
		}
	}
}
