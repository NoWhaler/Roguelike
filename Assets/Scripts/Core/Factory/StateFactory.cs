using System.Collections.Generic;
using Core._StateMachine;
using Game.Characters.Player.CharacterAnimation;
using Game.Characters.Player.Enum;
using Game.UI;
using Game.UI.Enum;
using Game.UI.UIStates;

namespace Core.Factory
{
    public class StateFactory
    {
        public StateMachine StateMachineContext { get; set; }

        private readonly Dictionary<UIState, BaseState<UIStateMachine>> _uiStates = new();

        private readonly Dictionary<CharacterAnimationState, BaseState<CharacterStateMachine>> _characterStates = new();

        public void InitUIStates()
        {
            _uiStates[UIState.Menu] = new MenuState(StateMachineContext as UIStateMachine, this);
            _uiStates[UIState.Gameplay] = new GameplayState(StateMachineContext as UIStateMachine, this);
            _uiStates[UIState.Settings] = new SettingsState(StateMachineContext as UIStateMachine, this);
        }

        public void InitCharacterStates()
        {
            _characterStates[CharacterAnimationState.Idle] =
                new CharacterIdleState(StateMachineContext as CharacterStateMachine, this);
            _characterStates[CharacterAnimationState.Walk] =
                new CharacterWalkState(StateMachineContext as CharacterStateMachine, this);
            _characterStates[CharacterAnimationState.Attack] =
                new CharacterAttackState(StateMachineContext as CharacterStateMachine, this);
        }

        public BaseState<UIStateMachine> Menu()
        {
            return _uiStates[UIState.Menu];
        }

        public BaseState<UIStateMachine> Settings()
        {
            return _uiStates[UIState.Settings];
        }

        public BaseState<UIStateMachine> Gameplay()
        {
            return _uiStates[UIState.Gameplay];
        }

        public BaseState<CharacterStateMachine> Idle()
        {
            return _characterStates[CharacterAnimationState.Idle];
        }

        public BaseState<CharacterStateMachine> Walk()
        {
            return _characterStates[CharacterAnimationState.Walk];
        }

        public BaseState<CharacterStateMachine> Attack()
        {
            return _characterStates[CharacterAnimationState.Attack];
        }
    }
}