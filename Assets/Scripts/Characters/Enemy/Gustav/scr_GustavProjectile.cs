using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class scr_GustavProjectile : MonoBehaviour {


	[SerializeField] float speed = 10.0f;
	[SerializeField] Vector2 direction = new Vector2 (-1, 0);
	[SerializeField] float damage = 10;
	[SerializeField] float timeToLive = 10; //em Segundos
	[SerializeField] float force = 10;

	private Rigidbody2D entityRigidBody;
	[SerializeField]
	scr_AudioClient audioClient;
	private bool explode = false;
	//Nome da Tag do dono para impedir friendly Fire
	private string ownerTag;


	void Awake(){
		this.entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		this.entityRigidBody.velocity = this.direction.normalized * speed;

		this.Fire (new Vector2 (-1, -1), "Enemy");
	}
		
	/// <summary>
	/// Fire the in the specified fireDirection and ownerTag.
	/// </summary>
	/// <param name="fireDirection">Fire direction.</param>
	/// <param name="ownerTag">Owner tag.</param>
	public void Fire (Vector2 fireDirection, string ownerTag){
		this.ownerTag = ownerTag;
		this.direction = fireDirection;
		//print ("" + direction);

		if (this.direction.x < 1)
			GetComponent<SpriteRenderer> ().flipX = true;
		
		this.entityRigidBody.velocity = this.direction.normalized * speed;
		audioClient.playAudioClip ("Flyby", scr_AudioClient.sources.local);
	}

	/// <summary>
	/// Fires in a straight line towards a target
	/// </summary>
	/// <param name="targetTransform">Target transform.</param>
	/// <param name="ownerTag">Owner tag.</param>
	public void FireStraight (Transform targetTransform, string ownerTag){
		this.direction = targetTransform.position - transform.position;
		if (this.direction.x < 1)
			GetComponent<SpriteRenderer> ().flipX = true;

		this.entityRigidBody.velocity = this.direction.normalized * speed;
		audioClient.playAudioClip ("Flyby", scr_AudioClient.sources.local);
	}

		
	public void Update(){
	}

	void Die(){
		
		//Destroy (this.gameObject);
		//Destroy (this);
	}

	void OnCollisionEnter2D(Collision2D col){

		/*if (col.gameObject.tag == ownerTag) {
			Physics2D.IgnoreCollision (col.gameObject.GetComponent<Collider2D> (), 
				this.gameObject.GetComponent<Collider2D> (), true);
		}

		//Entidade "danificável"
		scr_HealthController entity = col.gameObject.GetComponent<scr_HealthController> ();
		if (entity != null && entity.tag != ownerTag) {
			entity.takeDamage (this.damage, this.direction.normalized * force);
		}*/
		explode = true;
		Die ();
	}

	void OnTriggerStay2D(Collision2D col){
		if (explode && !col.gameObject.CompareTag(ownerTag)) {
			scr_HealthController health = col.gameObject.GetComponent<scr_HealthController> ();
			if (health != null) {
				health.takeDamage (this.damage, this.direction.normalized * force);
			}
		}
	}



}
