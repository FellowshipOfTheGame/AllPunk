using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Particle_Emitters : MonoBehaviour {

	public string nameManager = "GustavBattleManager";
	public enum Instant
	{
		chase, gun, locomotive
	};
	public Instant battleInstant = Instant.chase;
	public scr_AudioClient audioClient;

	private scr_Gustav_Battle_Manager batMan;
	private ParticleSystem particles;

	void Awake() {
		particles = GetComponent<ParticleSystem>();
	}

	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.Find(nameManager);
		if(manager != null) {
			batMan = manager.GetComponent<scr_Gustav_Battle_Manager>();
			if(batMan != null)
				batMan.addParticleEmitter(this);
		}
		else{
			Debug.LogWarning("Can't find battle manager");
		}
	}
	
	public void activateParticles(){
		particles.Stop();
		particles.Play();
	}

	public void stopParticles(){
		particles.Stop();
	}

	public void activateForTime(float time){
		particles.Stop();
		ParticleSystem.MainModule main = particles.main;

		main.duration = time/2;
		main.startLifetime = time/2;
		particles.Play();

		if(audioClient != null) {
			audioClient.playLocalClip("Steam");
		}
	}

}
