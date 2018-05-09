using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EP_JetJump : scr_EP {

	#region variables
	[Header("Jet Jump Variables")]
	[SerializeField] scr_PlayerController playerController;
	[SerializeField] Rigidbody2D playerRigidbody;
	[SerializeField] scr_PlayerEnergyController playerEnergy;

	[SerializeField] float impulseMagnitude;
	[SerializeField] int numberOfJumps;
	[SerializeField] ParticleSystem particles;

	private int currNumberOfJumps;
	#endregion


	public override bool Equip (GameObject playerReference)
	{
		playerController = playerReference.GetComponent<scr_PlayerController> ();
		playerRigidbody = playerReference.GetComponent<Rigidbody2D> ();
		playerEnergy = playerReference.GetComponent<scr_PlayerEnergyController> ();
		currNumberOfJumps = numberOfJumps;

		if (playerController != null && playerRigidbody != null && playerEnergy != null)
			return true;
		else
			return false;
	}

	public override bool Unequip ()
	{
		playerController = null;
		playerRigidbody = null;
		playerEnergy = null;
		return true;
	}
		
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Jump") && !playerController.isGrounded && currNumberOfJumps > 0) {
			///Kills Y velocity before applying Impulse to make jumping uniform, without it, the
			///height of the jump became erratic, eg. doublejumping from the ground gave the most height.
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
			playerRigidbody.AddForce (Vector2.up * impulseMagnitude, ForceMode2D.Impulse);
			currNumberOfJumps--;
			playerEnergy.drainEnergy (energyDrain);
		} else if (playerController.isGrounded)
			currNumberOfJumps = numberOfJumps;

		
	}


}
