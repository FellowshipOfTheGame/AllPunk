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
	[SerializeField] scr_EPManager epMan;

	[Header("References")]
	[Tooltip("Referencia para placas da perna direita")]
	public Transform RPauldron;
	[Tooltip("Posição para placas da perna direita")]
	public Vector3 RPaulPosition;
	[Tooltip("Rotação para placas da perna direita")]
	public Vector3 RPaulRotation;
	[Tooltip("Particulas da perna direita")]
	public ParticleSystem RParticles;
	[Tooltip("Posição das particulas da perna direita")]
	public Vector3 RPartPosition;


	[Tooltip("Referencia para placas da perna esquerda")]
	public Transform LPauldron;
	[Tooltip("Posição para placas da perna esquerda")]
	public Vector3 LPaulPosition;
	[Tooltip("Rotação para placas da perna esquerda")]
	public Vector3 LPaulRotation;
	[Tooltip("Particulas da perna esquerda")]
	public ParticleSystem LParticles;
	[Tooltip("Posição das particulas da perna esquerda")]
	public Vector3 LPartPosition;


	public Vector3 PartRotationR;
	public Vector3 PartRotationL;

	private bool isRight = true;
	private int currNumberOfJumps;
	#endregion


	public override bool Equip (GameObject playerReference)
	{
		playerController = playerReference.GetComponent<scr_PlayerController> ();
		playerRigidbody = playerReference.GetComponent<Rigidbody2D> ();
		playerEnergy = playerReference.GetComponent<scr_PlayerEnergyController> ();
		currNumberOfJumps = numberOfJumps;

		Transform hipTransform = playerReference.transform.Find("Bones").Find("Hip");
		if(hipTransform != null){
			RPauldron.SetParent(hipTransform);
			RPauldron.localPosition = RPaulPosition;
			RPauldron.localRotation = Quaternion.Euler(RPaulRotation);
			LPauldron.SetParent(hipTransform);
			LPauldron.localPosition = LPaulPosition;
			LPauldron.localRotation = Quaternion.Euler(LPaulRotation);

			Transform leg = hipTransform.Find("R.UpperLeg");
			RParticles.transform.SetParent(leg);
			RParticles.transform.localPosition = RPartPosition;
			RParticles.transform.localRotation = Quaternion.Euler(PartRotationR);

			leg = hipTransform.Find("L.UpperLeg");
			LParticles.transform.SetParent(leg);
			LParticles.transform.localPosition = LPartPosition;
			LParticles.transform.localRotation = Quaternion.Euler(PartRotationR);


		}

		epMan = playerReference.GetComponent<scr_EPManager>();
		epMan.addFlipCallback(flip);

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

		epMan.removeFlipCallback(flip);

		Destroy(RPauldron);
		Destroy(RParticles);
		Destroy(LPauldron);
		Destroy(LParticles);
		
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
			audioClient.playRandomClip (scr_AudioClient.sources.sfx);

			//Particuals
			if(isRight ^ playerController)
			RParticles.Play();
			LParticles.Play();

		} else if (playerController.isGrounded)
			currNumberOfJumps = numberOfJumps;

		
	}

	public void flip() {
		isRight = !isRight;
		Quaternion rotation = (isRight) ? Quaternion.Euler(PartRotationR) : Quaternion.Euler(PartRotationL);
		LParticles.transform.localRotation = rotation;
		RParticles.transform.localRotation = rotation;
	}

}
