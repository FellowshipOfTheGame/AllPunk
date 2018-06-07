using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
	/// <summary>
	/// State class of the StateMachine.
	/// Each state will contain its own implementation of the
	/// required methods, store all connected states to it
	///  and will be responsable for checking if a State Transition must occur
	/// </summary>
	public abstract class State : MonoBehaviour
	{
		[Header("State Variables")]
		/// <summary>
		/// Reference to the linked state machine.
		/// </summary>
		[SerializeField]
		protected StateMachine stateMachine;
		/// <summary>
		/// Dictionary including all connected States.
		/// </summary>
		[SerializeField]
		protected StateDictionary connectedStates;

		/// <summary>
		/// To be executed when the FSM enters this Method
		/// </summary>
		public abstract void Enter();
		/// <summary>
		/// To be executed everytime the FSM UpdateState() is called
		/// It will verify if a new State needs to be entered, if not
		/// it will run the code 
		/// </summary>
		public abstract void Execute();
		/// <summary>
		/// To be executed when the FSM exits this Method
		/// </summary>
		public abstract void Exit();
	}
}

