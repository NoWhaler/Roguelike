using Core._StateMachine;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    public class UIStateMachine: StateMachine, IInitializable, ITickable
    {
        public Canvas MenuCanvas { get; set; }
        
        public Canvas SettingsCanvas { get; set; }
        
        public Canvas GameplayCanvas { get; set; }
        
        public bool IsMenuActive { get; set; }
        
        public bool IsSettingsActive { get; set; }
        
        public bool IsGameplayActive { get; set; }

        public BaseState<UIStateMachine> CurrentState { get; set; }
        
        public void Initialize()
        {
            StateFactory.StateMachineContext = this;
            StateFactory.InitUIStates();
            
            CurrentState = StateFactory.Gameplay();
            
            CurrentState.EnterState();
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                IsMenuActive = true;
            }

            else if (Input.GetKeyDown(KeyCode.F2))
            {
                IsGameplayActive = true;
            }

            else if (Input.GetKeyDown(KeyCode.F3))
            {
                IsSettingsActive = true;
            }
            
            CurrentState.UpdateStates();
        }
    }
}