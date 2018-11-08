using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class scr_GustavProjectile : MonoBehaviour {


	[SerializeField] float speed = 10.0f;
	[SerializeField] Vector2 direction = new Vector2 (-1, 0);
	[SerializeField] float damage = 10;
	[SerializeField] float timeToLive = 10; //em Segundos
	[SerializeField] float force = 10;
	[SerializeField] public GameObject explosionPrefab;
	[SerializeField] float explosionTimeToDestroy = 1;

	private Rigidbody2D entityRigidBody;
	[SerializeField]
	scr_AudioClient audioClient;
	private bool explode = false;
	//Nome da Tag do dono para impedir friendly Fire
	private string damageTag;


	void Awake(){
		this.entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		//this.entityRigidBody.velocity = this.direction.normalized * speed;

		//this.Fire (new Vector2 (-1, -1), "Enemy");
	}
		
	/// <summary>
	/// Fire the in the specified fireDirection and damageTag.
	/// </summary>
	/// <param name="fireDirection">Fire direction.</param>
	/// <param name="damageTag">Owner tag.</param>
	public void Fire (Vector2 fireDirection, string damageTag){
		this.damageTag = damageTag;
		this.direction = fireDirection;

		this.direction.Normalize ();
		LookAt (direction);

		if (this.direction.x < 1)
			GetComponent<SpriteRenderer> ().flipX = true;
		
		this.entityRigidBody.velocity = this.direction * speed;
		//audioClient.playAudioClip ("Flyby", scr_AudioClient.sources.local);
	}

	/// <summary>
	/// Fires in a straight line towards a target
	/// </summary>
	/// <param name="targetTransform">Target transform.</param>
	/// <param name="damageTag">Damage tag.</param>
	public void FireStraight (Transform targetTransform, string damageTag){
		this.damageTag = damageTag;
		this.direction = targetTransform.position - transform.position;

		this.direction.Normalize ();
		LookAt (direction);
		//if (this.direction.x < 1)
		//	GetComponent<SpriteRenderer> ().flipX = true;

		this.entityRigidBody.velocity = this.direction * speed;
		audioClient.playAudioClip ("Flyby", scr_AudioClient.sources.local);

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0,0,angle);
	}

		
	public void Update(){
	}

	void Die(){
		GameObject explosion = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		Destroy(explosion, explosionTimeToDestroy);

		audioClient.playLocalClip("Explode");

		Destroy(this.gameObject);
		//Destroy (this.gameObject);
		//Destroy (this);
	}

	void OnCollisionEnter2D(Collision2D col){

		/*if (col.gameObject.tag == damageTag) {
			Physics2D.IgnoreCollision (col.gameObject.GetComponent<Collider2D> (), 
				this.gameObject.GetComponent<Collider2D> (), true);
		}

		//Entidade "danificável"
		scr_HealthController entity = col.gameObject.GetComponent<scr_HealthController> ();
		if (entity != null && entity.tag != damageTag) {
			entity.takeDamage (this.damage, this.direction.normalized * force);
		}*/

		explode = true;
		if (explode && col.gameObject.CompareTag(damageTag)) {
			scr_HealthController health = col.gameObject.GetComponent<scr_HealthController> ();
			if (health != null) {
				health.takeDamage (this.damage, this.direction.normalized * force);
			}
		}
		//explode = true;
		Die ();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
			Die();
	}

	void LookAt(Vector2 lookAtDirection){
		float rot_z = Mathf.Atan2 (lookAtDirection.y, lookAtDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (0f, 0f, rot_z );
	}


}