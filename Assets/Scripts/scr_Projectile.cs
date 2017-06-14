using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Projectile : MonoBehaviour {


	public float speed = 10.0f;
	public Vector2 direction = new Vector2 (-1, 0);
	public float damage = 10;

	private Rigidbody2D entityRigidBody;


	void Awake(){
		this.entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		this.entityRigidBody.velocity = this.direction.normalized * speed;
	}
		

	public void Fire (Vector2 fireDirection){
		this.direction = fireDirection;
		this.entityRigidBody.velocity = this.direction.normalized * speed;
	}

	void Die(){
		Destroy (this.gameObject);
		Destroy (this);
	}

	void OnCollisionEnter2D(Collision2D col){
		/**
		 * QUESTIONAMENTO: Dependendo de como ficará a ideia da classe abstrata entity
		 * em algo mais voltado a unity, simplesmente fazer a verificação 
		 * col.gameObject é do tipo entity? Aplica dano
		 */
		switch (col.gameObject.tag){
		case "Enemy":
			scr_Enemy enemy = col.gameObject.GetComponent<scr_Enemy> ();
			enemy.takeDamage (this.damage, new Vector2 (10, 0));
			Die ();
			break;
		case "Player":
			scr_PlayerController player = col.gameObject.GetComponent<scr_PlayerController> ();
			//player.takeDamage (this.damage, new Vector2 (10, 0));
			Die ();
			break;		
		}
		if (col.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			Die ();
		}
	}
}
