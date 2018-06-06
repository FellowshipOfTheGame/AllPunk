using System;

namespace FSM
{
	public interface IState
	{
		void Enter();
		void Reason();
		void Act();
		void Exit();
	}
}

