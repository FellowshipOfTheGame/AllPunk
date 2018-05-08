using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EP : MonoBehaviour {

	public enum EpType
	{
		Head,
		Torso,
		Arm,
		Legs
	};

	#region Variables
	protected string keyName;
	protected string name;
	protected string descritption;
	protected EpType type;
	protected int energyDrain;
	protected int meshId;
	protected Sprite thumbImg;
	#endregion 

	#region getters

	public string getKeyName(){
		return keyName;
	}
	public string geName(){
		return name;
	}
	public string getDescription(){
		return descritption;
	}
	public EpType getEpType(){
		return type;
	}
	public int getEnergyDrain(){
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
