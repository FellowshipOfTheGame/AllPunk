using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Acess_GameManager : MonoBehaviour {

	private void startAnimation () {
		GetComponent<Animator>().SetTrigger("Trigger");
	}

	public void setPauseGame(bool pause) {
		scr_GameManager.instance.setPauseGame(pause);
		scr_HUDController.hudController.canPause = !pause;
	}

	public void hideHUD() {
		scr_GameManager.instance.setHudVisible(false);
	}

	public void showHUD() {
		scr_GameManager.instance.setHudVisible(true);
	}

	public void pauseGame() {
		setPauseGame(true);
	}

	public void unpauseGame() {
		setPauseGame(false);
		//removeFadeOut();
	}

	public void returnToMenu(){
		scr_GameManager.instance.goToMainMenu();
	}

	public void removeFadeOut() {
		
	}
}
