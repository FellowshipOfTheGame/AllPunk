﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Projectile : MonoBehaviour {


	public float speed = 10.0f;
	public Vector2 direction = new Vector2 (-1, 0);
	public float damage = 10;
	public float timeToLive = 10; //em Segundos

	private Rigidbody2D entityRigidBody;

	//Nome da Tag do dono para impedir friendly Fire
	private string ownerTag;


	void Awake(){
		this.entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		this.entityRigidBody.velocity = this.direction.normalized * speed;
	}
		

	public void Fire (Vector2 fireDirection, string ownerTag){
		this.ownerTag = ownerTag;
		this.direction = fireDirection;
		this.entityRigidBody.velocity = this.direction.normalized * speed;
	}

	public void Update(){
		if (timeToLive <= 0)
			Die ();
		else
			timeToLive -= Time.deltaTime;
	}

	void Die(){
		Destroy (this.gameObject);
		Destroy (this);
	}

	void OnCollisionEnter2D(Collision2D col){
		//Entidade "danificável"
		scr_HealthController entity = col.gameObject.GetComponent<scr_HealthController> ();
		if (entity != null && entity.tag != ownerTag) {
			entity.takeDamage (this.damage, this.direction.normalized);
		}
		Die ();

		/*if (col.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			Die ();
		}*/
	}
}
