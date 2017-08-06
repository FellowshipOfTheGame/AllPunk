using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_RivetGun : scr_Weapon
{
    public GameObject projectilePrefab;

	//Raio de distância entre a posição do player e o tiro
	[Range(2,4)]
	public float rangedAttackOffset = 3.0f;
	private Transform spawnPosition;//Posição para spawnar projétil

	public void Awake()
    {
        base.Awake();
        spawnPosition = transform.Find("SpawnPosition");
    }

    override protected void AttackAction(bool noAnimation) {
        if (clicked) {

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


			Vector3 weaponDirection = mouseWorldPosition - lowerArm.position;

			//POSSIBILIDADE: Sempre calcular o vetor WeaponDirection e se for menor do que 
			//determinado raio utilizar o valor antigo?


			//Spawn bullet
			GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPosition.position + 
				weaponDirection.normalized*rangedAttackOffset , spawnPosition.rotation);
			scr_Projectile projScript = proj.GetComponent<scr_Projectile>();
			projScript.Fire(weaponDirection, "Player");
			StartAttackAnimation();

        }
    }
}
