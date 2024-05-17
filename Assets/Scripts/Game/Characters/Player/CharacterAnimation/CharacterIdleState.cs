using Core._StateMachine;
using Core.Factory;

namespace Game.Characters.Player.CharacterAnimation
{
    public class CharacterIdleState: BaseState<CharacterStateMachine>
    {
        public CharacterIdleState(CharacterStateMachine currentStateContext, StateFactory stateFactory) :
            base(currentStateContext, stateFactory)
        {
        }

        public override void EnterState()
        {
        }

        protected override void UpdateState()
        {
            
        }

        protected override void ExitState()
        {
        }

        protected override void CheckSwitchStates()
        {
            throw new System.NotImplementedException();
        }
    }
}