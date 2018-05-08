using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_Equip_Button : MonoBehaviour {

    public scr_PA_Manager.PartType partType;
    public int partID;
    public bool isRight;
    private scr_SaveStation saveStation;
    private Text textScr;

    protected void Awake()
    {
        Button bt = GetComponent<Button>();
        bt.onClick.AddListener(this.whenClicked);
        textScr = GetComponentInChildren<Text>();
    }

    public void setProperties(scr_PA_Manager.PartType type, int id, scr_SaveStation saveRef, bool isRight)
    {
        setProperties(type, id, saveRef);
        this.isRight = isRight;
    }

    public void setProperties(scr_PA_Manager.PartType type, int id, scr_SaveStation saveRef)
    {
        partID = id;
        partType = type;
        saveStation = saveRef;
        textScr = GetComponentInChildren<Text>();
        Button bt = GetComponent<Button>();
        bt.onClick.AddListener(this.whenClicked);

        switch (type)
        {
            case scr_PA_Manager.PartType.Head:
                textScr.text = ((scr_PA_Manager.HeadPart)id).ToString();
                break;
            case scr_PA_Manager.PartType.Legs:
                textScr.text = ((scr_PA_Manager.LegPart)id).ToString();
                break;
            case scr_PA_Manager.PartType.Torso:
                textScr.text = ((scr_PA_Manager.TorsoPart)id).ToString();
                break;
            case scr_PA_Manager.PartType.Weapon:
                scr_PA_Manager.WeaponPart weapon = (scr_PA_Manager.WeaponPart)id;
                textScr.text = weapon.ToString();
                break;
        }
    }

    
    private void whenClicked()
    {
        if(saveStation != null)
        {
            saveStation.equipPart(partType, partID, isRight);
        }
    }

}
