using Core.Factory;
using Zenject;

namespace Core._StateMachine
{
    public abstract class BaseState<T> where T: StateMachine
    {
        protected StateFactory StateFactory { get; set; }

        protected T StateContext { get; set; }

        protected BaseState(T currentStateContext, StateFactory stateFactory)
        {
            StateContext = currentStateContext;
            StateFactory = stateFactory;
        }

        public abstract void EnterState();

        protected abstract void UpdateState();

        protected abstract void ExitState();

        protected abstract void CheckSwitchStates();

        public void UpdateStates()
        {
            UpdateState();
        }

        protected void SwitchState(BaseState<T> newState, ref BaseState<T> currentState)
        {
            ExitState();
            
            newState.EnterState();

            currentState = newState;
        }
    }
}