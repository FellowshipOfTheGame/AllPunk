using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Interactor : MonoBehaviour {

    private Collider2D selfCol;
    private bool canInteract = false;
    private List<scr_Interactable> interactables;

    public void Awake()
    {
        selfCol = GetComponent<Collider2D>();
        interactables = new List<scr_Interactable>();
    }

    /**
     * Place objects in interactables list
     */
    public void SetInteractable(scr_Interactable interactable) {
        this.canInteract = true;
        interactables.Add(interactable);
    }

    /**
     * Remove the object from the interactables list
     */
    public void RemoveInteractable(scr_Interactable interactable) {
        interactables.Remove(interactable);
        if (interactables.Count < 1) {
            canInteract = false;
        }
    }

    //Interact with the last interactable on lists
    void Update () {
        if (canInteract) {
            if (Input.GetKeyDown(KeyCode.F)) {
                interactables[interactables.Count-1].Interact(this);
            }
        }
	}
}
