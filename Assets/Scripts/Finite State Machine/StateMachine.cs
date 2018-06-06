using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
	/// <summary>
	/// State machine.
	/// It will only serve to call the transition methods and set the currentState variable
	/// It should NOT contain any logic regarding the states
	/// </summary>
	public class StateMachine
	{

		[SerializeField]
		State currentState;
		public State CurrentState {
			get {
				return this.currentState;
			}
		}

		/// <summary>
		/// Transitions the currentState to the nextState
		/// </summary>
		/// <param name="newState">New state.</param>
		public void transitionToState(State newState){
			///Error Checking
			if (currentState == null) {
				Debug.LogError ("FSM with Null CurrentState");
				return;
			}
			if (newState == null) {
				Debug.LogError ("FSM with Null NewState");
				return;
			}

			//Transition
			currentState.Exit ();
			currentState = newState;
			currentState.Enter ();
		}

		/// <summary>
		/// Updates the state.
		/// </summary>
		public void UpdateState(){
			currentState.Execute ();
		}
	}
}

