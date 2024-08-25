using Core._StateMachine;
using Core.Factory;
using UnityEngine;

namespace Game.UI.UIStates
{
    public class MenuState: BaseState<UIStateMachine>
    {
        public MenuState(UIStateMachine currentStateContext, StateFactory stateFactory) :
            base(currentStateContext, stateFactory)
        {
        }

        public override void EnterState()
        {
        }

        protected override void UpdateState()
        {
            CheckSwitchStates();
        }

        protected override void ExitState()
        {
            StateContext.IsMenuActive = false;
        }

        protected override void CheckSwitchStates()
        {
            var stateContextCurrentState = StateContext.CurrentState;
            
            if (StateContext.IsGameplayActive)
            {
                SwitchState(StateFactory.Gameplay(), ref stateContextCurrentState);
                
                StateContext.CurrentState = stateContextCurrentState;
            }
            
            else if (StateContext.IsSettingsActive)
            {
                SwitchState(StateFactory.Settings(), ref stateContextCurrentState);
                
                StateContext.CurrentState = stateContextCurrentState;
            }
        }
    }
}