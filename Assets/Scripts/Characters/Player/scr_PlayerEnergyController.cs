using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerEnergyController : MonoBehaviour {

	[Header("Sigma's Reserve Energy Stats")]
	[Tooltip("Maximum Reserve Energy")]
	private float currResEnergy;
	[SerializeField] float maxResEnergy;
	[SerializeField] float reserveRechargeRate;

	[Header("Sigma's Primary Energy Stats")]
	private float currPrimEnergy;
	[SerializeField] float maxPrimEnergy;
	[SerializeField] scr_Rechargable primEnergySource;
		
	public float getMaxResEnergy(){
		return maxResEnergy;
	}

	public float getCurrentResEnergy(){
		return currResEnergy;
	}

	public float getResRechargeRate(){
		return reserveRechargeRate;
	}

	public float getTotalCurrentEnergy(){
		return currResEnergy + currPrimEnergy;
	}

	public void setCurrentResEnergy(float newEnergy) {
		currResEnergy = newEnergy;
		if(currResEnergy > maxResEnergy)
			currResEnergy = maxResEnergy;
		if(currResEnergy < 0)
			currResEnergy = 0;
	}

	public void setMaxResEnergy(float max){
		maxResEnergy = max;
	}

	public void setResRechargeRate(float rate){
		reserveRechargeRate = rate;
	}

	public void setPrimEnergySource (scr_Rechargable rec){
		if (rec != null) {
			primEnergySource = rec;

		}
		//PASSAR COMO CALLBACK A FUNCAO DE RECARGA PARA ENERGIA PRINCIAPL
	}

	public void setMaxPrimEnergy(float max){
		maxPrimEnergy = max;
		if (currPrimEnergy > maxPrimEnergy)
			currPrimEnergy = maxPrimEnergy;
	}


	/// <summary>
	/// Drains the energy, first the re	serve, then the primary.
	/// </summary>
	/// <param name="drain">Energy to Drain.</param>
	/// <returns>True if energy was drained, false if insuficient energy</returns>
	public bool drainEnergy(float drain){
		//Reserve energy alone is enough
		if (currResEnergy >= drain) {
			currResEnergy -= drain;
			return true;
		}
		//Requires 
		else if (currResEnergy + currPrimEnergy >= drain) {
			currResEnergy -= drain;
			if (currResEnergy < 0) {
				currPrimEnergy += currResEnergy;
				currResEnergy = 0;
			}
			return true;
		} else
			return false;
	}

	// Use this for initialization
	void Awake () {
		currResEnergy = maxResEnergy;
	}
	
	// Update is called once per frame
	void Update () {

		if (currResEnergy > maxResEnergy)
			currResEnergy = maxResEnergy;

		//print ("en: " + currResEnergy + "/" + maxResEnergy);
		if (currResEnergy + reserveRechargeRate < maxResEnergy)
			currResEnergy += reserveRechargeRate*Time.timeScale;
		else if (currResEnergy < maxResEnergy)
			currResEnergy += maxResEnergy-currResEnergy*Time.timeScale;
	}
}
