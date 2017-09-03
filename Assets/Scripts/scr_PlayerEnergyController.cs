using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerEnergyController : MonoBehaviour {

	private float currentEnergy;
	public float maxEnergy;
	public float rechargeRate;


	public float getMaxEnergy(){
		return maxEnergy;
	}

	public float getCurrentEnergy(){
		return currentEnergy;
	}

	public void drainEnergy(float drain){
		currentEnergy -= drain;
		if (currentEnergy < 0)
			currentEnergy = 0;
	}

	// Use this for initialization
	void Awake () {
		currentEnergy = maxEnergy;
	}
	
	// Update is called once per frame
	void Update () {
		print ("en: " + currentEnergy + "/" + maxEnergy);
		if (currentEnergy + rechargeRate < maxEnergy)
			currentEnergy += rechargeRate;
		else
			currentEnergy += maxEnergy-currentEnergy;
	}
}
