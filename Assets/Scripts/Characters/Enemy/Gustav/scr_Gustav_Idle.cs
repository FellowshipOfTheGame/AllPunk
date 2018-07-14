using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Idle : FSM.State {

	public scr_Gustav_Battle_Manager battleManager;

	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		battleManager = GetComponent<scr_Gustav_Battle_Manager>();
	}

	public override void Enter (){

	}

	public override void Execute () {
		if(battleManager.hasReachedStart){
			FSM.State state;
			bool sucess = connectedStates.TryGetValue("Running", out state);
			if(sucess){
				stateMachine.transitionToState(state);
			}
			else{
				Debug.LogWarning("Can't find next state");
			}
			
		}
	}

	public override void Exit (){
		//Começa a mover o cenário

	}
}
