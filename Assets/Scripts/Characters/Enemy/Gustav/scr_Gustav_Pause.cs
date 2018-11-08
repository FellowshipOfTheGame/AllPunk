using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Pause : FSM.State {

	public scr_Gustav_Battle_Manager battleManager;
	public float runningSpeed = 40;
	public float transitionTime = 2;

	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		battleManager = GetComponent<scr_Gustav_Battle_Manager>();
	}

	public override void Enter (){
		//Começa a mover o cenário
		battleManager.setBackgroundSpeed(runningSpeed, transitionTime);

	}

	public override void Execute () {
		if (battleManager.hasReachedLocomotive){
			FSM.State state;
			bool sucess = connectedStates.TryGetValue("Locomotive", out state);
			if(sucess){
				stateMachine.transitionToState(state);
			}
			else{
				Debug.LogWarning("Can't find next state");
			}
		}
		else if(battleManager.hasReachedEnd && battleManager.lifePorcentGun > 0){
			FSM.State state;
			bool sucess = connectedStates.TryGetValue("Battle", out state);
			if(sucess){
				stateMachine.transitionToState(state);
			}
			else{
				Debug.LogWarning("Can't find next state");
			}
			
		}
	}

	public override void Exit (){
	}
}
