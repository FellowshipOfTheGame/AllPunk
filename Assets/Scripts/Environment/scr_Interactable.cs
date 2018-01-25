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
	protected void Awake () {
		interactionZone = GetComponent<Collider2D> ();
        interactors = new List<scr_Interactor>();
	}
	
	void OnTriggerEnter2D(Collider2D col){
        scr_Interactor inter = null;

        inter = col.GetComponent<scr_Interactor>();
        print("DEBUG: " + col.gameObject.name);
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

    /// <summary>
    /// Função chamada pela entidade que está interagindo com o objeto
    /// </summary>
    /// <param name="interactor">Entidade interagindo com o objeto</param>
    /// <returns></returns>
    public abstract bool Interact(scr_Interactor interactor);

    /// <summary>
    /// Função chamada quando o objeto se torna interagivel
    /// </summary>
    protected abstract void BecameInterable();

    /// <summary>
    /// Função chamada quando o objeto deixa de ser interagivel
    /// </summary>
    protected abstract void StopInterable();

    protected void OnDestroy()
    {
        foreach (scr_Interactor inter in interactors) {
            inter.RemoveInteractable(this);
        }
    }
}
