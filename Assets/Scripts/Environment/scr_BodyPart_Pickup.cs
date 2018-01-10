using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class scr_BodyPart_Pickup : MonoBehaviour
{

    public int partID = -1;

    /**
     * Diferent part bodies that can be equiped
     */
    public enum bodyPart
    {
        LeftArm,
        RightArm,
        AnyArm,
        Head,
        Body,
        Legs
    }

    public enum EquipUnlock
    {
        Unlock,
        Equip,
        EquipAndUnlock
    };

    //Parte do corpo que deve ser equipada
    public bodyPart bodyPartToEquip = bodyPart.AnyArm;

    public EquipUnlock equipOrUnlock = EquipUnlock.Unlock;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            scr_PA_Manager paMan = collision.gameObject.GetComponent<scr_PA_Manager>();
            if (paMan != null)
            {
                //Se for necessário desbloquear
                if(equipOrUnlock != EquipUnlock.Equip) {
                    if(bodyPartToEquip == bodyPart.Body)
                        paMan.unlockPart(scr_PA_Manager.PartType.Torso, partID);
                    else if(bodyPartToEquip == bodyPart.Head)
                        paMan.unlockPart(scr_PA_Manager.PartType.Head, partID);
                    else if(bodyPartToEquip == bodyPart.Legs)
                        paMan.unlockPart(scr_PA_Manager.PartType.Legs, partID);
                    else
                        paMan.unlockPart(scr_PA_Manager.PartType.Weapon, partID);
                }
                //Se for necessario equipar
                if(equipOrUnlock != EquipUnlock.Unlock) {
                    switch (bodyPartToEquip)
                    {
                        /**
                        *  ID da arma para equipar, a variavel arm pode ser:
                        *  -1: Don't care
                        *  0: No braço direito
                        *  1: No braço esquerdo
                        */
                        case bodyPart.LeftArm:
                            paMan.equipWeapon(partID, 1);
                            break;
                        case bodyPart.RightArm:
                            paMan.equipWeapon(partID, 0);
                            break;
                        case bodyPart.AnyArm:
                            paMan.equipWeapon(partID, -1);
                            break;
                        case bodyPart.Head:
                            paMan.equipHead(partID);
                            break;
                        case bodyPart.Body:
                            paMan.equipBody(partID);
                            break;
                        case bodyPart.Legs:
                            paMan.equipLegs(partID);
                            break;
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
