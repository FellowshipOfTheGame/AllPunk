using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Projectile : MonoBehaviour {


	public float speed = 10.0f;
	public Vector2 direction = new Vector2 (-1, 0);
	public float damage = 10;
	public float timeToLive = 10; //em Segundos
    public float force = 10;

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
		//print ("" + direction);

		this.direction.Normalize ();
		LookAt (direction);

		if (this.direction.x > 1)
			GetComponent<SpriteRenderer> ().flipX = true;
		
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

		if (col.gameObject.tag == ownerTag) {
			Physics2D.IgnoreCollision (col.gameObject.GetComponent<Collider2D> (), 
				this.gameObject.GetComponent<Collider2D> (), true);
		}

		//Entidade "danificável"
		scr_HealthController entity = col.gameObject.GetComponent<scr_HealthController> ();
		if (entity != null && entity.tag != ownerTag) {
			entity.takeDamage (this.damage, this.direction.normalized * force);
		}
		Die ();

		/*if (col.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			Die ();
		}*/
	}

	void LookAt(Vector2 lookAtDirection){
		float rot_z = Mathf.Atan2 (lookAtDirection.y, lookAtDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (0f, 0f, rot_z );
	}
}
