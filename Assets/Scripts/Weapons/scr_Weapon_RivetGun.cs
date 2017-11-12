using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_RivetGun : scr_Weapon
{
	
    public GameObject projectilePrefab;
	//Raio de distância entre a posição do player e o tiro
	[Range(2,4)]
	public float rangedAttackOffset = 3.0f;
	public Transform spawnPosition;//Posição para spawnar projétil


	[Header("Particles")]
	public GameObject particlePuff;
	public GameObject particleLeak;

	public void Awake()
    {
        base.Awake();
		if(spawnPosition==null)
        	spawnPosition = transform.Find("SpawnPosition");
    }

    override protected void AttackAction(bool noAnimation) {
        if (clicked) {

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			Vector3 weaponDirection = mouseWorldPosition - lowerArm.position;
		
			if (particlePuff != null && particleLeak != null) {
				GameObject o = GameObject.Instantiate (particlePuff, this.transform, false);
				o.transform.SetParent (null);	

				int upAngle = 90;
				//Instancia o vazamento para cima e para baixo
				for (int i = 0; i < 2; i++) {
					upAngle = upAngle * -1;
					o = GameObject.Instantiate (particleLeak, this.transform, false);
					o.transform.Rotate (new Vector3 (upAngle, 0, 0));
					o.transform.SetParent (null);	
					o.GetComponent<ParticleSystem> ().Play ();
				}

			}

			//Spawn bullet
			GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPosition.position + 
				weaponDirection.normalized*rangedAttackOffset , spawnPosition.rotation);


			scr_Projectile projScript = proj.GetComponent<scr_Projectile>();
			projScript.Fire(weaponDirection, "Player");
			StartAttackAnimation();

        }
    }
}
