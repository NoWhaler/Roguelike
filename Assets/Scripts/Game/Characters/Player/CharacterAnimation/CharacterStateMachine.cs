using Core._StateMachine;
using Core.Factory;
using Zenject;

namespace Game.Characters.Player.CharacterAnimation
{
    public class CharacterStateMachine: StateMachine, IInitializable, ITickable
    {
        public string WalkAnimationHash { get; set; }

        public BaseState<CharacterStateMachine> CurrentState { get; set; }

        public void Initialize()
        {
            StateFactory.StateMachineContext = this;
            StateFactory.InitCharacterStates();
            
            CurrentState = StateFactory.Idle();

            CurrentState.EnterState();
        }
        
        public void Tick()
        {
            CurrentState.UpdateStates();
        }
    }
}