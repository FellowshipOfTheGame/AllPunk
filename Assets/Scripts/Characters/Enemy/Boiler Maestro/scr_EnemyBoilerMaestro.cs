﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyBoilerMaestro : MonoBehaviour {

	#region variables

	[SerializeField] FSM.StateMachine stateMachine;
	[SerializeField] CapsuleCollider2D capCollider;
	public Rigidbody2D rb2D;
	public Animator animator;

	private bool isFacingRight = true;

	[SerializeField]
	Transform groundCheckOrigin;

	//Used in the grounded check
	private RaycastHit2D groundHit;
	//Used in the obstacle check
	private RaycastHit2D obstacleHit;

	//Height of the capsuleCollider, used in grounded check
	private float height;
	//Used to RayCast for obstacles
	private Vector2 groundHitPoint = Vector2.zero;

	/// <summary>
	/// The player target.
	/// </summary>
	private GameObject target = null;
	public GameObject Target {
		get {
			return this.target;
		}
		set {
			target = value;
		}
	}
	#endregion

	#region MonoBehavior methods
	void Awake(){
		if (stateMachine == null)
			stateMachine = GetComponent<FSM.StateMachine> ();
		if (animator == null)
			animator = GetComponent<Animator> ();
		if(rb2D == null)
			rb2D = GetComponent<Rigidbody2D> ();
		if (capCollider == null)
			capCollider = GetComponent<CapsuleCollider2D> ();
		if (groundCheckOrigin == null)
			transform.Find ("GroundCheckOrigin");
	}

	void Start(){
		height = capCollider.size.y;
	}

	void FixedUpdate(){
		stateMachine.UpdateState ();
	}

	#endregion


	#region Sensor Methods
	void OnTriggerEnter2D(Collider2D col){
		if (col.CompareTag ("Player")) {
			target = col.gameObject;
		}
	}
	void OnTriggerExit2D(Collider2D col){
		if (col.CompareTag ("Player")) {
			target = null;
		}
	}
		

	/// <summary>
	/// Checks if the enemy is in contact with an collider of the "Ground" Layer.
	/// </summary>
	/// <returns><c>true</c>, if grounded, <c>false</c> otherwise.</returns>
	public bool isGrounded(){

		//Debug.DrawLine (transform.position, transform.position + new Vector3 (0, -height, 0), Color.cyan);

		///With the bitwise shift left of the layerMask, any object NOT IN THE GROUND layer will be filtered OUT
		//groundHit = Physics2D.Raycast (transform.position + height, Vector2.down, height, 1 << LayerMask.NameToLayer("Ground"));
		groundHit = Physics2D.Raycast (groundCheckOrigin.position, Vector2.down, height*1.5f, LayerMask.GetMask("Ground"));

		if (groundHit.collider != null) {
			return true;
		}
		else
			return false;
	}


	/// <summary>
	/// Checks if there is a collider of the "Ground" Layer directly ahead of the Enemy
	/// </summary>
	/// <returns><c>true</c>, if there is <c>false</c> otherwise.</returns>
	public bool hasFloor(){
		//Raycasts towards the ground in front of the enemy
		if(isFacingRight)
			///With the bitwise shift left of the layerMask, any object NOT IN THE GROUND layer will be filtered OUT
			groundHit = Physics2D.Raycast (transform.position + Vector3.right*height + Vector3.down, Vector2.down, height*2,
				LayerMask.GetMask("Ground"));
		else
			groundHit = Physics2D.Raycast (transform.position + Vector3.left*height  + Vector3.down, Vector2.down, height*2,
				LayerMask.GetMask("Ground"));
		
		if (groundHit.collider != null) {
			groundHitPoint = groundHit.point;
			return true;
		}
		else
			return false;
	}


	/// <summary>
	/// Checks if there is an obstacle in front of the enemy.
	/// Takes the position of the last hasFloor rayCastHit and
	/// Raycasts upwards to check if there's enough space for the
	/// enemy
	/// </summary>
	/// <returns><c>true</c>, if obstacle <c>false</c> otherwise.</returns>
	public bool hasObstacle(){
		//Raises the point a bit to avoid colliding with the floor
		groundHitPoint += Vector2.up * 0.1f;
		Debug.DrawLine (groundHitPoint, groundHitPoint + Vector2.up * height, Color.cyan);

		//Ignore all collisions not on the Default or Ground Layers
		//int layerMask = (1 << LayerMask.NameToLayer ("Default") | 1 << LayerMask.NameToLayer ("Ground"));
		obstacleHit = Physics2D.Raycast (groundHitPoint, Vector2.up, height, LayerMask.GetMask("Ground","Default"));

		if (obstacleHit.collider != null) {
			print("> " + obstacleHit.transform);
			return true;
		}
		else
			return false;
	}

	public bool IsFacingRight {
		get {
			return this.isFacingRight;
		}
	}
	#endregion

	#region Action Methods
	/// <summary>
	/// Flips this instance. 
	/// Alters isFacingRight as well as the sprite
	/// </summary>
	public void Flip()
	{
		// Switch the way the player is labeled as facing.
		isFacingRight = !isFacingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	/// <summary>
	/// Faces the target. Target needs to be null
	/// </summary>
	public void faceTarget(){
		if (Target != null) {
			if (Target.transform.position.x < transform.position.x && IsFacingRight)
				Flip ();
			else if (Target.transform.position.x > transform.position.x && !IsFacingRight)
				Flip ();
		}
	}

	/// <summary>
	/// Moves horizontally, towards the way its facing
	/// </summary>
	/// <param name="speed">Speed.</param>
	public void horizontalMove(float speed){
		if(IsFacingRight)
			rb2D.velocity = new Vector2(1 * speed, rb2D.velocity.y);
		else
			rb2D.velocity = new Vector2(-1 * speed, rb2D.velocity.y);

		animator.SetBool("IsGrounded", isGrounded());
		animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb2D.velocity.x));
		animator.SetFloat("VerticalSpeed", rb2D.velocity.y);
	}

	#endregion
}
