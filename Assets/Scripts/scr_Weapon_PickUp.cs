using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class scr_Weapon_PickUp : MonoBehaviour {

    public int weaponID = -1;
    /**
     *  ID da arma para equipar, a variavel arm pode ser:
     *  -1: Don't care
     *  0: No braço direito
     *  1: No braço esquerdo
     */
    public int armToEquip;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            scr_PA_Manager paMan = collision.gameObject.GetComponent<scr_PA_Manager>();
            if (paMan != null) {
                paMan.equipWeapon(weaponID, armToEquip);
                Destroy(gameObject);
            }
        }
    }
}
