using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EP_Steam_Goggles : scr_EP {

    public override bool Equip(GameObject playerReference)
    {
        scr_HUDController.hudController.setPlayerHasGoggles(true);
		return true;
    }

    public override bool Unequip()
    {
        scr_HUDController.hudController.setPlayerHasGoggles(false);
		return true;
    }
}
