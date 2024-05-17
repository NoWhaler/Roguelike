using Core.Factory;
using UnityEngine;
using Zenject;

namespace Core._StateMachine
{
    public abstract class StateMachine: MonoBehaviour
    {
        protected StateFactory StateFactory { get; set; }
        
        [Inject]
        public void Constructor(StateFactory stateFactory)
        {
            StateFactory = stateFactory;
        }
    }
}