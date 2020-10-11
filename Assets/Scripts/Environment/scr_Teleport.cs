 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Teleport : MonoBehaviour
{
    public Transform teleportPosition;

    private GameObject playerReference;

    private bool isTeleporting = false;

    void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Player" && !isTeleporting) {
            isTeleporting = true;
            playerReference = col.gameObject;
			scr_HUDController.hudController.fadeIn(finishedFading);
			scr_HUDController.hudController.canPause = false;
			scr_GameManager.instance.setPauseGame(true);
		}
	}

	private void finishedFading(){
        playerReference.transform.position = teleportPosition.position;
        StartCoroutine(StartFadeOut());
	}

    private IEnumerator StartFadeOut()
    {
        yield return null;
        scr_HUDController.hudController.fadeOut(null);
        scr_HUDController.hudController.canPause = true;
        scr_GameManager.instance.setPauseGame(false);
        isTeleporting = false;
    }

    private void Placebo()
    {
        //Não espera nada, é só efeito placebo
    }
}
