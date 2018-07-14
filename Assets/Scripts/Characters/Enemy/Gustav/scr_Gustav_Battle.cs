using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Battle : FSM.State {

	public scr_Gustav_Battle_Manager battleManager;

	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		battleManager = GetComponent<scr_Gustav_Battle_Manager>();
	}

	public override void Enter (){
		Debug.Log("MODO BATALHA");
	}

	public override void Execute () {

	}

	public override void Exit (){
		
	}
}
