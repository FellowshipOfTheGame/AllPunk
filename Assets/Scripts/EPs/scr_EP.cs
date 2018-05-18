using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class scr_EP : MonoBehaviour {

	public enum EpType
	{
		Head,
		Torso,
		Arm,
		Legs
	};

	#region Variables
	[SerializeField]
	protected string keyName;
	[SerializeField]
	protected string epName;
	[SerializeField]
	protected string descritption;
	[SerializeField]
	protected EpType type;
	[SerializeField]
	protected float energyDrain;
	[SerializeField]
	protected int meshId;
	[SerializeField]
	protected Sprite thumbImg;
	#endregion 

	#region getters

	public string getKeyName(){
		return keyName;
	}
	public string geEpName(){
		return epName;
	}
	public string getDescription(){
		return descritption;
	}
	public EpType getEpType(){
		return type;
	}
	public float getEnergyDrain(){
		return energyDrain;
	}
	public int getMeshId(){
		return meshId;
	}
	public Sprite getThumbImg(){
		return thumbImg;
	}

	#endregion

	/// <summary>
	/// Equips the EP. use playerReference to get any variable used in the player, if needed.
	/// </summary>
	/// <param name="playerReference">Player reference.</param>
	abstract public bool Equip(GameObject playerReference);

	/// <summary>
	/// Unequip this instance.
	/// </summary>
	abstract public bool Unequip();
}
