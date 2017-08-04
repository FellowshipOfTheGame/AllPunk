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

    private void Awake()
    {
        base.Awake();
        spawnPosition = transform.Find("SpawnPosition");
    }

    override protected void AttackAction(bool noAnimation) {
        if (noAnimation && clicked) {

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 weaponDirection = mouseWorldPosition - spawnPosition.position;

			//Spawn bullet
			GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPosition.position + weaponDirection.normalized*rangedAttackOffset , spawnPosition.rotation);
			scr_Projectile projScript = proj.GetComponent<scr_Projectile>();
			projScript.Fire(weaponDirection);
			StartAttackAnimation();

        }
    }
}
