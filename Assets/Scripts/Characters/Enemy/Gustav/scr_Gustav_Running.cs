using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Running : FSM.State {

	public scr_Gustav_Battle_Manager battleManager;

	[Header("Running atributes")]
	[Tooltip("Tempo para subir depois de descer")]
	public float timerUp;
	[Tooltip("Tempo para descer depois que subiu")]
	public float timerDown;
	[Tooltip("Tempo até soltar barulho de tiro")]
	public float timerStartShoot;
	[Tooltip("Tempo para atirar, depois que soltou o barulho do tiro")]
	public float timerFallShoot;
	[Tooltip("Quantos ciclos de subir e descer precisam para que ele atire")]
	public float ciclesTillFire = 2;


	private enum SubState
	{
		Up, Down, PlayShoot, FallShoot
	};

	private SubState currentState;
	private SubState nextState;

	private float timer;
	private int currentCicle = 0;

	private void Awake() {
		stateMachine = GetComponent<FSM.StateMachine>();
		battleManager = GetComponent<scr_Gustav_Battle_Manager>();
	}

	public override void Enter (){
		currentState = SubState.Down;
		nextState = SubState.Up;
		timer = timerUp;
		currentCicle = 0;
	}

	public override void Execute () {
		//Realiza a transição para o modo de batalha
		if(battleManager.hasReachedEnd) {
			FSM.State state;
			bool sucess = connectedStates.TryGetValue("Battle", out state);
			if(sucess){
				stateMachine.transitionToState(state);
			}
			else{
				Debug.LogWarning("Can't find next state");
			}
		} 
		//Realiza lógica
		else {
			//Timer estorou
			if(timer <= 0){
				//Muda o estado
				currentState = nextState;
				if(currentState == SubState.Up){
					battleManager.elevate();
					currentCicle++;
					if(currentCicle >= ciclesTillFire){
						nextState = SubState.PlayShoot;
						timer = timerStartShoot;
					}
					else{
						nextState = SubState.Down;
						timer = timerDown;
					}
				}
				else if(currentState == SubState.PlayShoot){
					currentCicle = 0;
					battleManager.playSound();
					nextState = SubState.FallShoot;
					timer = timerFallShoot;
				}
				else if(currentState == SubState.FallShoot) {
					battleManager.shoot();
					nextState = SubState.Down;
					float toTime = timerDown - (timerFallShoot+timerStartShoot);
					if(toTime < 0)
						toTime = 0;

					timer = toTime;
				}
				else if(currentState == SubState.Down) {
					battleManager.descent();
					nextState = SubState.Up;
					timer = timerUp;
				}
			}
		}
		timer -= Time.fixedDeltaTime;
	}

	public override void Exit (){
		battleManager.descent();
	}

}
