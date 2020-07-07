using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class scr_PlayerEnergyController : MonoBehaviour {

	#region variables
	[Header("Sigma's Reserve Energy Stats")]
	[Tooltip("Maximum Reserve Energy")]
	private float currResEnergy;
	[SerializeField] float maxResEnergy;
	[SerializeField] float reserveRechargeRate;
	/// <summary>
	/// The time in seconds without using the primary energy to begin the recharge
	/// </summary>
	[SerializeField] float rechargeWarmUp = 1f;

	[Header("Sigma's Primary Energy Stats")]

	/// <summary>
	/// The current primary energy. Public to allow Torso EPs to recharge it
	/// </summary>
	public float currPrimEnergy;
	[SerializeField] float maxPrimEnergy;
		

	/// <summary>
	/// Used to check if the reserve energy can be recharged
	/// </summary>
	private bool canRechargeReserve = true;

	private Coroutine reserveRechargeCoroutine;

	/// <summary>
	/// The change callback, used to update the HUD
	/// </summary>
	UnityEvent energyChangeCallback;
	#endregion

	#region getters/setters
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

	public float getCurrentPrimEnergy(){
		return currPrimEnergy;
	}

	public float getMaxPrimEnergy(){
		return maxPrimEnergy;
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

	/// <summary>
	/// Sets the max prim energy. MUST be called by torso EPs on Equiping
	/// </summary>
	/// <param name="max">Max.</param>
	public void setMaxPrimEnergy(float max){
		maxPrimEnergy = max;
		if (currPrimEnergy > maxPrimEnergy)
			currPrimEnergy = maxPrimEnergy;
	}
	#endregion


	#region Monobehaviour Methods
	// Use this for initialization
	void Awake () {
		currResEnergy = maxResEnergy;
		currPrimEnergy = currPrimEnergy;
		if (energyChangeCallback == null) {
			energyChangeCallback = new UnityEvent();
		}
	}

	// Update is called once per frame
	void Update () {

		if (canRechargeReserve) {
			currResEnergy = Mathf.Clamp (currResEnergy + reserveRechargeRate * Time.deltaTime, 0, maxResEnergy);

			if(energyChangeCallback != null)
				energyChangeCallback.Invoke ();
		}
	}

	private void OnDestroy()
	{
		if(energyChangeCallback != null)
		{
			energyChangeCallback.RemoveAllListeners();
		}

	}

	#endregion


	/// <summary>
	/// Drains the energy, first the re	serve, then the primary.
	/// </summary>
	/// <param name="drain">Energy to Drain.</param>
	/// <returns>True if energy was drained, false if insuficient energy</returns>
	public bool drainEnergy(float drain){
		//Reserve energy alone is enough
		if (currResEnergy >= drain) {
			currResEnergy -= drain;
			if(reserveRechargeCoroutine != null)
				StopCoroutine(reserveRechargeCoroutine);
			reserveRechargeCoroutine = StartCoroutine (reserveRechargeWarmUp ());
			if(energyChangeCallback != null)
				energyChangeCallback.Invoke ();
			return true;
		}
		//Requires 
		else if (currResEnergy + currPrimEnergy >= drain) {
			currResEnergy -= drain;
			if (currResEnergy < 0) {
				currPrimEnergy += currResEnergy;
				currResEnergy = 0;
			}
			if(reserveRechargeCoroutine != null)
				StopCoroutine(reserveRechargeCoroutine);
			reserveRechargeCoroutine = StartCoroutine (reserveRechargeWarmUp ());
			return true;
		} else
			return false;
	}


	IEnumerator reserveRechargeWarmUp(){
		canRechargeReserve = false;
		yield return new WaitForSeconds (rechargeWarmUp);
		canRechargeReserve = true;
	}

	#region callBackMethods
	public void addEnergyChangeCallback(UnityAction call)
	{
		if(energyChangeCallback != null)
			energyChangeCallback.AddListener(call);
	}

	public void removeEnergyChangeCallback(UnityAction call)
	{
		if(energyChangeCallback != null)
			energyChangeCallback.RemoveListener(call);
	}
	#endregion

}
