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

    //Parte do corpo que deve ser equipada
    public bodyPart bodyPartToEquip = bodyPart.AnyArm;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            scr_PA_Manager paMan = collision.gameObject.GetComponent<scr_PA_Manager>();
            if (paMan != null)
            {
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
                Destroy(gameObject);
            }
        }
    }
}
