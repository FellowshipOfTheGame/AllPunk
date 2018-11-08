using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Energy_Pickup : MonoBehaviour {

	public float energyToRecover;
	public string boylerKey = "torso_boiler";
	
	private void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == "Player"){
			scr_EPManager epMan = other.gameObject.GetComponent<scr_EPManager>();
			if(epMan != null) {
				if(epMan.getCurrentPart(scr_EP.EpType.Torso) == boylerKey){
					scr_EP_Boiler playerBoiler = (scr_EP_Boiler)epMan.getCurrentPartRef (scr_EP.EpType.Torso);
					playerBoiler.burnCoal(energyToRecover);
				}
			}

			scr_AudioClient audio = GetComponent<scr_AudioClient>();
			if(audio != null){
				audio.playAudioClip("RecoverEnergy", scr_AudioClient.sources.sfx);
			}

			Destroy(gameObject);
		}
	}
}
