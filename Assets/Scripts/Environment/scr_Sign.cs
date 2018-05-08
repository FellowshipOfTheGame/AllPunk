using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Object that acts like a sign, showing a text when the player walks by
 */
public class scr_Sign : scr_Interactable {

    public Canvas canvas;

    public void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    public override bool Interact(scr_Interactor interactor) {
        Destroy(interactor.gameObject);
        return true;
    }

    protected override void BecameInterable()
    {
        canvas.enabled = true;
    }

    protected override void StopInterable()
    {
        canvas.enabled = false;
    }
}
