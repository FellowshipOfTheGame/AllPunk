using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_SteamBreath : scr_Weapon {

	#region variables

	public float meleeAtackDistance = 1.0f;
	public float knockbackIntensity;
	public float timeToFire = 1.0f;
	public GameObject smokePrefab;//Usado para instanciar a fumaça

	private Transform spawnPosition;//Posição para spawnar hitbox
	/*private float currentTimeToFire;*/

	/*IEnumerator fireTimer(float timeToFire){
		//yield return new WaitForSeconds (this.timeToFire);
		yield return new WaitForSeconds (1.0f);
		currentTimeToFire = 0;
	}*/
	#endregion variables

	public void Awake()
	{
		base.Awake();
		//currentTimeToFire = 0;

	}

	/*private void Update(){
		base.Update ();
		if (currentTimeToFire > 0) {
			currentTimeToFire -= Time.deltaTime;
			//print ("~ " + currentTimeToFire);
			if (currentTimeToFire <= 0)
				currentTimeToFire = 0;
		}
	}*/

	override protected void AttackAction(bool noAnimation) {
	/**
	 * Para o steam breath, projetar uma Hitbox que realizará dano
	 * zero e dará knockback em um vetor na direção mirada
	 */
		if (clicked) {
			
			//StartCoroutine (fireTimer(this.timeToFire));

			spawnPosition = transform.Find("SpawnPosition");

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 weaponDirection = mouseWorldPosition - upperArm.position; 

			Vector2 pos = new Vector2 (spawnPosition.position.x + weaponDirection.normalized.x,
				              spawnPosition.position.y + weaponDirection.normalized.y);

			Collider2D[] hits = new Collider2D[10];
			PolygonCollider2D collider = GetComponent<PolygonCollider2D>();//Referencia para o collider 
			ContactFilter2D ct2D = new ContactFilter2D();
			collider.OverlapCollider(ct2D, hits);

			GameObject.Instantiate(smokePrefab, pos, transform.rotation);

			foreach (Collider2D hit in hits) {

				scr_HealthController entity = hit.GetComponent<scr_HealthController> ();
				if (entity != null && entity.tag != "Player") {
					print (entity);
					//entity.takeDamage (0, weaponDirection.normalized * knockbackIntensity);new Vector2 (weaponDirection.x, weaponDirection.y)
					//print("VecWDir " + weaponDirection);
					//print("VecKnock " + weaponDirection * knockbackIntensity * knockbackIntensity);
					entity.takeDamage (0, weaponDirection.normalized * knockbackIntensity);
				}
			}
			//StartAttackAnimation();
		}
	}
}
