using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Battle : FSM.State {

	public scr_Gustav_Battle_Manager battleManager;
	[Tooltip("Quantidade de inimigos spawnados por wave")]
	public int spawnQuantity = 5;
	[Tooltip("Tempo entre spawns")]
	public float spawnTimer = 10;

	private float timer;

	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		battleManager = GetComponent<scr_Gustav_Battle_Manager>();
		
	}

	public override void Enter (){
		Debug.Log("MODO BATALHA");
		timer = 0;
	}

	public override void Execute () {
		if(battleManager.lifePorcent > 0){
			timer -= Time.deltaTime;
			if(timer <= 0) {
				battleManager.spawnBattleWave(spawnQuantity);
				timer = spawnTimer;
			}
		}
		else {
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
	}
}
