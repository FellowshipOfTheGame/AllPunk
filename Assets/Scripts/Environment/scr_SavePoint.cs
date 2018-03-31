using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scr_SavePoint : scr_Interactable {

	[Tooltip("Esse save point recupera a vida do jogador")]
	public bool recoverHP = true;
	[Tooltip("Esse save point recupera a energia do jogador")]
	public bool recoverEnergy = false;
	private Canvas canvas;
	private Text text;


	protected void Awake() {
		base.Awake();
		canvas = transform.Find("Canvas").GetComponent<Canvas>();
		text = canvas.GetComponentInChildren<Text>();
		canvas.enabled = false;
	}

    public override bool Interact(scr_Interactor interactor)
    {
		string newText = "Saved with sucess";
		//Garantir que é o jogador		
		if(interactor.gameObject.tag != "Player")
			return false;
		scr_GameManager.instance.updatePlayerStats();
        scr_Player_Stats playerStats = scr_GameManager.instance.playerStats;
		if(recoverHP) {
			playerStats.currentHp = playerStats.maxHp;
			scr_HealthController health = interactor.GetComponent<scr_HealthController>();
			health.setCurrentHealth(health.getMaxHealth());
			newText += "\nHealth recovered!";
		}
		if(recoverEnergy) {
			playerStats.currentResEnergy = playerStats.maxResEnergy;
			scr_PlayerEnergyController energy = interactor.GetComponent<scr_PlayerEnergyController>();
			energy.setCurrentResEnergy(energy.getMaxResEnergy());
			newText += "\nEnergy recovered!";
		}
		playerStats.savePointScene = SceneManager.GetActiveScene().name;
		playerStats.savePointName = gameObject.name;
		scr_GameManager.instance.playerStats = playerStats;
		bool result = scr_GameManager.instance.Save();
		if(result && text != null)
			text.text = newText;
		return result;

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
