using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Hacks : MonoBehaviour
{
    [MenuItem("Hacks/Equip full kit")]
    public static void EquipFullKit()
    {
        if(!Application.isPlaying) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        scr_EPManager epMan = player.GetComponent<scr_EPManager>();

        if(epMan != null)
        {
            epMan.unlockPart("arm_gatling_gun");
            epMan.unlockPart("legs_jet_jump");
            epMan.unlockPart("torso_boiler");
            epMan.unlockPart("arm_fire_sword");

            epMan.equipPart("arm_gatling_gun", scr_EPManager.ArmToEquip.LeftArm);
            epMan.equipPart("arm_fire_sword", scr_EPManager.ArmToEquip.RightArm);
            epMan.equipPart("torso_boiler");
            epMan.equipPart("legs_jet_jump");
        }

        scr_PlayerEnergyController ener = player.GetComponent<scr_PlayerEnergyController>();
        if(ener != null)
        {
            ener.setCurrentPrimEnergy(ener.getMaxPrimEnergy());
        }
    }
}
