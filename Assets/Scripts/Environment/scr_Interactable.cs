using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Abstract class of interactable objects
 */
public abstract class scr_Interactable : MonoBehaviour {

	protected Collider2D interactionZone;
    //List of all interactors inside the interaction
    protected List<scr_Interactor> interactors;
    //If it has any interactor inside
    protected bool inside = false;

	//Initialize the list and components
	void Awake () {
		interactionZone = GetComponent<Collider2D> ();
        interactors = new List<scr_Interactor>();
	}
	
	void OnTriggerEnter2D(Collider2D col){
        scr_Interactor inter;

        inter = col.GetComponent<scr_Interactor>();
        
        //Add the interactor object to the list if valid
        if (inter != null) {
            interactors.Add(inter);
            //Tell interactor that this is a valid object
            inter.SetInteractable(this);
            if (!inside)
            {
                inside = true;
                BecameInterable();
            }
        }


    }

	void OnTriggerExit2D(Collider2D col){
        scr_Interactor inter;
        inter = col.GetComponent<scr_Interactor>();

        //Remove the interactible if it's valid
        if (inter != null) {
            interactors.Remove(inter);
            inter.RemoveInteractable(this);
            //Set the variable if list is empty
            if (interactors.Count < 1)
            {
                inside = false;
                StopInterable();
            }
        }
	}

    /**
     * Function called by interactor to interact with the object. Take the interactor by parameter
     */
    public abstract bool Interact(scr_Interactor interactor);

    /**
     * Function called when object became interable
     */
    protected abstract void BecameInterable();

    /**
     * Function called when object stop being interable
     */
    protected abstract void StopInterable();

    protected void OnDestroy()
    {
        foreach (scr_Interactor inter in interactors) {
            inter.RemoveInteractable(this);
        }
    }
}
