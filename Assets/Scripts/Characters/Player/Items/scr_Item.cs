using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface with methods required for all usable items
/// </summary>
public interface scr_Item {

	#region AudioVisual Methods
	/// <summary>
	/// Gets the item sprite. Used for the UI
	/// </summary>
	/// <returns>The item sprite.</returns>
	Sprite getItemSprite();

	/// <summary>
	/// Gets the audio clip to be played when the item is usedd
	/// </summary>
	/// <returns>The audio clip.</returns>
	scr_AudioClipWrapper getAudioClip ();
	#endregion

	#region Quantity Methods
	/// <summary>
	/// Gets the current item quanity.
	/// </summary>
	/// <returns>The curr qty.</returns>
	int getCurrQty ();

	/// <summary>
	/// Sets the new curr qty.
	/// </summary>
	/// <param name="newQty">New qty.</param>
	void setCurrQty(int newQty);

	/// <summary>
	/// Gets the max qty.
	/// </summary>
	/// <returns>The max qty.</returns>
	int getMaxQty ();
	#endregion

	/// <summary>
	/// Uses the item.
	/// </summary>
	/// <returns><c>true</c>, if item was used, <c>false</c> otherwise.</returns>
	bool useItem ();

	/// <summary>
	/// Sets the player references, if required by the item implementation
	/// </summary>
	/// <param name="playerObject">Player object.</param>
	void setPlayerReferences(GameObject playerObject);
}
