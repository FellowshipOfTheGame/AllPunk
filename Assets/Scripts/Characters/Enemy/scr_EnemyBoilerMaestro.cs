using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyBoilerMaestro : MonoBehaviour {

	private enum state{
		Wander,
		WalkTowards,
		JumpUp,
		JumpDown,
		Smash,
		Charge
	};

	private state currState;

	// Use this for initialization
	void Start () {
		currState = state.Wander;
	}
	
	// Update is called once per frame
	void Update () {
		switch (currState) {
		case state.Wander:
			Wander ();
			break;
		}
	}

	public void Wander(){
	
	}
		
}
