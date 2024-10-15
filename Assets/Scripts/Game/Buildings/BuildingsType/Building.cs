using System.Collections.Generic;
using Core.TurnBasedSystem;
using Game.Buildings.Enum;
using Game.Researches.Controller;
using Game.WorldGeneration.Hex;
using UnityEngine;
using Zenject;

namespace Game.Buildings.BuildingsType
{
    public abstract class Building: MonoBehaviour
    {
        [field: SerializeField] public BuildingType BuildingType { get; set; }
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }
        
        [SerializeField] private HexModel _currentHex;

        private Collider _buildingCollider;
        
        protected List<IBuildingAction> _availableActions = new List<IBuildingAction>();

        protected ResearchesController _researchesController;

        protected HexGridController _hexGridController;

        protected GameTurnController _gameTurnController;

        [Inject]
        private void Constructor(GameTurnController gameTurnController,
            ResearchesController researchesController, HexGridController hexGridController)
        {
            _gameTurnController = gameTurnController;
            _researchesController = researchesController;
            _hexGridController = hexGridController;
        }

        public virtual void Initialize()
        {
            CurrentHealth = MaxHealth;
            SetupActions();
        }
        
        public List<IBuildingAction> GetAvailableActions()
        {
            return _availableActions;
        }
        
        public void AddAction(IBuildingAction action)
        {
            _availableActions.Add(action);
        }

        public void RemoveAction(IBuildingAction action)
        {
            _availableActions.Remove(action);
        }
        
        public List<HexModel> GetNeighboringHexes()
        {
            List<HexModel> neighbors = new List<HexModel>();
            
            int[,] directions = new int[,] {
                {1, 0, -1}, {1, -1, 0}, {0, -1, 1},
                {-1, 0, 1}, {-1, 1, 0}, {0, 1, -1}
            };

            for (int i = 0; i < 6; i++)
            {
                int neighborQ = _currentHex.Q + directions[i, 0];
                int neighborR = _currentHex.R + directions[i, 1];
                int neighborS = _currentHex.S + directions[i, 2];

                HexModel neighborHex = GetHexAtCoordinates(neighborQ, neighborR, neighborS);
                if (neighborHex != null)
                {
                    neighbors.Add(neighborHex);
                }
            }

            return neighbors;
        }
     
        private HexModel GetHexAtCoordinates(int q, int r, int s)
        {
            return _hexGridController.GetHexAt(q, r, s);
        }
        
        protected abstract void SetupActions();
    }
}