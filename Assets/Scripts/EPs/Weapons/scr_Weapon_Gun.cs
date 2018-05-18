using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_Gun : scr_Weapon
{
    public GameObject projectilePrefab;
    public float intensity;
    private Transform spawnPosition;


	public override bool Unequip ()
	{
		Debug.Log("Removed: "+ epName);
		return true;
	}

    private void Awake()
    {
        base.Awake();
        spawnPosition = transform.Find("SpawnPosition");
    }

    override protected void AttackAction(bool noAnimation) {
        if (noAnimation && clicked) {
            StartAttackAnimation();

            //Spawn bullet
            GameObject bullet = GameObject.Instantiate(projectilePrefab, spawnPosition.position, spawnPosition.rotation);
            scr_Projectile proj = bullet.GetComponent<scr_Projectile>();
            Vector3 direction = spawnPosition.position - transform.position;
            direction = direction.normalized * intensity;
            proj.Fire(direction, "Player");
        }
    }
}
