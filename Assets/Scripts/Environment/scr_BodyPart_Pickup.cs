using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class scr_BodyPart_Pickup : MonoBehaviour
{

    public enum EquipUnlock
    {
        Unlock,
        Equip,
        EquipAndUnlock
    };

    public string EPKey;

    public EquipUnlock equipOrUnlock = EquipUnlock.Unlock;

    public scr_EPManager.ArmToEquip arm;
      
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            scr_EPManager EPMan = collision.gameObject.GetComponent<scr_EPManager>();
            if (EPMan != null)
            {
                //Se for necessário desbloquear
                if(equipOrUnlock != EquipUnlock.Equip) {
                    EPMan.unlockPart(EPKey);
                }
                //Se for necessario equipar
                if(equipOrUnlock != EquipUnlock.Unlock) {
                    EPMan.equipPart(EPKey, arm);
                }
                Destroy(gameObject);
            }
        }
    }
}
