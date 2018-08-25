using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_HealthKit : MonoBehaviour {

	[Tooltip("Quantidade a ser recuperada de vida")]
	public float healthRecovery;

	private void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.tag == "Player"){
			scr_HealthController health = other.gameObject.GetComponent<scr_HealthController>();
			if(health != null) {
				health.removeDamage(healthRecovery);
			}

			scr_AudioClient audio = GetComponent<scr_AudioClient>();
			if(audio != null){
				audio.playAudioClip("RecoverHealth", scr_AudioClient.sources.sfx);
			}

			Destroy(gameObject);
		}
	}

}
