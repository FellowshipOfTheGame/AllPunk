using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Locomotive : FSM.State {

	public scr_Gustav_Battle_Manager battleManager;

	[Header("Steam spawn")]
	[Tooltip("Intervalo entre qual pode acontecer vapor")]
	public float minRandTime = 5;
	[Tooltip("Intervalo entre qual pode acontecer vapor")]
	public float maxRandTime = 15;
	[Tooltip("Duração do vapor")]
	public float duration = 5;
	[Tooltip("Velocidade com que a tela embaça sem o óculos")]
	public float alphaToCondensate = 0.5f;
	[Tooltip("Quantidade de bagpipers para spawnar")]
	public int spawnQuatity = 5;

	public float timeToStop = 10;

	private float timer;

	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		battleManager = GetComponent<scr_Gustav_Battle_Manager>();
		
	}

	public override void Enter (){
		Debug.Log("MODO BATALHA");
		timer = 0;
		battleManager.startSteamCoroutine(scr_Gustav_Particle_Emitters.Instant.locomotive,minRandTime,maxRandTime,duration,alphaToCondensate,spawnQuatity);
	}

	public override void Execute () {
		if(battleManager.lifePorcentLocomotive <= 0){
			FSM.State state;
			bool sucess = connectedStates.TryGetValue("Dead", out state);
			if(sucess){
				stateMachine.transitionToState(state);
			}
			else{
				Debug.LogWarning("Can't find next state");
			}
		}
	}

	public override void Exit (){
		//Parar cenário
		Debug.Log("Batalha acabou!");
		battleManager.stopSteamCoroutine();
		battleManager.spawnExplosionLocomotive();
		battleManager.locomotivePlataform.resumePlataform();
		battleManager.setBackgroundSpeed(0, timeToStop);
		battleManager.locomotiveParticle.Stop();
	}
}
