using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Battle_Manager : MonoBehaviour {

	public bool hasReachedStart = false;
	public bool hasReachedEnd = false;
	public bool hasReachedPause = false;
	public GameObject bulletPrefab;
	public Vector3 bulletOffset;
		
	public float lifePorcent;
	public scr_Gustav_Background_Scroller scroller;
	public scr_HealthController healthController;
	public scr_AudioClient bulletSoundClient;

	[Header("Spawn Steam and Bats")]
	public GameObject batpipperPrefab;
	public Vector3 spawnOffset;
	public float steamDuration;
	public float spawnSteamRandomMin = 5;
	public float spawnSteamRandomMax = 15;
	public float alphaCondesation = 0.5f;
	public scr_Gustav_Particle_Emitters.Instant steamSpawnInstant;
	[Tooltip("The porcentage of the duration when it should spawn the bats")]
	[Range(0,1)]
	public float momentToSpawnBats = 0.75f;
	public int spawnSteamQuantity = 2;
	private bool shouldSpawnSteam = false;
	private float currentSteamTarget;
	private float currentSteamCounter = 0;
	private Coroutine steamCoroutine = null;


	private FSM.StateMachine stateMachine;
	private List<scr_Gustav_Elevating_Plataform> plataforms;
	private List<scr_Gustav_Particle_Emitters> particle_Emitters;
	private GameObject player;


	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		plataforms = new List<scr_Gustav_Elevating_Plataform>();
		particle_Emitters = new List<scr_Gustav_Particle_Emitters>();
	}

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		healthController.addHealthChangeCallback(updateLife);

	}

	void FixedUpdate(){
		stateMachine.UpdateState ();
	}


	public void addPlataform(scr_Gustav_Elevating_Plataform plat) {
		if(plataforms != null)
			plataforms.Add(plat);
	}

	public void removePlataform(scr_Gustav_Elevating_Plataform plat) {
		if(plataforms != null)
			plataforms.Remove(plat);
	}

	public void addParticleEmitter(scr_Gustav_Particle_Emitters part){
		if(particle_Emitters != null)
			particle_Emitters.Add(part);
	}

	public void removeParticleEmitter(scr_Gustav_Particle_Emitters part){
		if(particle_Emitters != null)
			particle_Emitters.Remove(part);
	}

	public void setReachedStart(bool hasReached) {
		hasReachedStart = hasReached;
	}

	public void setReachedPauseStart(bool hasReached) {
		hasReachedPause = hasReached;
	}

	public void setReachedEnd(bool hasReached) {
		hasReachedEnd = hasReached;
	}

	public void elevate(){
		foreach (scr_Gustav_Elevating_Plataform plat in plataforms) {
			if(plat != null)
				plat.elevate();
		}
	}

	public void descent(){
		foreach (scr_Gustav_Elevating_Plataform plat in plataforms) {
			if(plat != null)
				plat.descent();
		}
	}

	public void playSound() {
		bulletSoundClient.playLocalClip("Shoot");
		Debug.Log("TOCA SOM");
	}

	public void setBackgroundSpeed(float destinySpeed, float timeDelta) {
		scroller.changeSpeed(destinySpeed,timeDelta);
	}

	public void setBackgroundImediateSpeed(float destinySpeed){
		scroller.imediateChangeSpeed(destinySpeed);
	}

	public void shoot() {
		GameObject projectile = GameObject.Instantiate(bulletPrefab,player.transform.position+bulletOffset, Quaternion.identity);
		scr_GustavProjectile pScript = projectile.GetComponent<scr_GustavProjectile>();
		pScript.FireStraight(player.transform, "Player");
	}

	public void spawnBattleWave(float quantity) {
		GameObject toSpawn;
		for(int i = 0; i < quantity; i++) {
			float range = Random.Range(-1,1);
			Vector3 positionToSpawn = new Vector3(spawnOffset.x*range,spawnOffset.y,0);
			toSpawn = GameObject.Instantiate(batpipperPrefab, player.transform.position + positionToSpawn, Quaternion.identity);
		}
	}

	public void updateLife() {
		lifePorcent = healthController.getCurrentHealth() / healthController.getMaxHealth();
	}

	public void startSteamCoroutine(scr_Gustav_Particle_Emitters.Instant instant, float minRandTime, float maxRandTime, float duration, float alphaToCondensate, int spawnQuatity){
		if(steamCoroutine != null)
			stopSteamCoroutine();
		spawnSteamRandomMin = minRandTime;
		spawnSteamRandomMax = maxRandTime;
		steamDuration = duration;
		spawnSteamQuantity = spawnQuatity;
		alphaCondesation = alphaToCondensate;
		shouldSpawnSteam = true;
		steamSpawnInstant = instant;
		steamCoroutine = StartCoroutine(spawnSteam());
	}

	public void stopSteamCoroutine(){
		if(steamCoroutine != null){
			shouldSpawnSteam = false;
			StopCoroutine(steamCoroutine);
			scr_HUDController.hudController.setPlayerInSteam(false, alphaCondesation);
			steamCoroutine = null;
		}
	}

	private IEnumerator spawnSteam(){
		while(shouldSpawnSteam){
			currentSteamTarget = Random.Range(spawnSteamRandomMin,spawnSteamRandomMax);
			yield return new WaitForSeconds(currentSteamTarget);
			StartSteam(steamSpawnInstant, steamDuration);
			//Make the hud start the steam effect
			scr_HUDController.hudController.setPlayerInSteam(true, alphaCondesation);
			yield return new WaitForSeconds(steamDuration * momentToSpawnBats);
			spawnBattleWave(spawnSteamQuantity);
			yield return new WaitForSeconds(steamDuration * (1-momentToSpawnBats));
			scr_HUDController.hudController.setPlayerInSteam(false, alphaCondesation);
		}
	}

	private void StartSteam(scr_Gustav_Particle_Emitters.Instant instant, float duration){
		foreach(scr_Gustav_Particle_Emitters pe in particle_Emitters){
			if(pe.battleInstant == instant){
				pe.activateForTime(duration);
			} else {
				pe.stopParticles();
			}
		}
	}

	private void StopSteam(){
		foreach(scr_Gustav_Particle_Emitters pe in particle_Emitters){
			pe.stopParticles();
		}
	}

}
