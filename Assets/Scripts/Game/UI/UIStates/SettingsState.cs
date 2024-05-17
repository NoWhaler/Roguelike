using Core._StateMachine;
using Core.Factory;
using UnityEngine;

namespace Game.UI.UIStates
{
    public class SettingsState: BaseState<UIStateMachine>
    {
        public SettingsState(UIStateMachine currentStateContext, StateFactory stateFactory) :
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
            StateContext.IsSettingsActive = false;
        }

        protected override void CheckSwitchStates()
        {
            var stateContextCurrentState = StateContext.CurrentState;
                        
            if (StateContext.IsMenuActive)
            {
                SwitchState(StateFactory.Menu(), ref stateContextCurrentState);
                
                StateContext.CurrentState = stateContextCurrentState;
            }
            
            else if (StateContext.IsGameplayActive)
            {
                SwitchState(StateFactory.Gameplay(), ref stateContextCurrentState);
                
                StateContext.CurrentState = stateContextCurrentState;
            }
        }
    }
}