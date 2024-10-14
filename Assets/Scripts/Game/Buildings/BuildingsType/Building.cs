using System.Collections.Generic;
using Game.Buildings.Enum;
using UnityEngine;

namespace Game.Buildings.BuildingsType
{
    public abstract class Building: MonoBehaviour
    {
        [field: SerializeField] public BuildingType BuildingType { get; set; }
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }

        private Collider _buildingCollider;
        
        protected List<IBuildingAction> _availableActions = new List<IBuildingAction>();

        public void Initialize()
        {
            CurrentHealth = MaxHealth;
            SetupActions();
        }
        
        public List<IBuildingAction> GetAvailableActions()
        {
            return _availableActions;
        }
        
        protected void AddAction(IBuildingAction action)
        {
            _availableActions.Add(action);
        }

        protected void RemoveAction(IBuildingAction action)
        {
            _availableActions.Remove(action);
        }
        
        protected abstract void SetupActions();
    }
}