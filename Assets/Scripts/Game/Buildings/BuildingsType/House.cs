using Game.Units.Enum;
using UnityEngine;

namespace Game.Buildings.BuildingsType
{
    public class House: Building
    {
        private const float MAX_CURSE_VALUE = 100f;
        private const float CURSE_CHANGE_RATE = 5f;
        private const float CURSE_THRESHOLD_POINT_OF_NO_RETURN = MAX_CURSE_VALUE;
        
        [field: SerializeField] public float CurrentCurseValue { get; private set; }
        
        private bool _hasReachedPointOfNoReturn;

        public override void Initialize()
        {
            base.Initialize();
            CurrentCurseValue = 0f;
            _hasReachedPointOfNoReturn = false;
            UpdateCurseState();
        }

        protected override void SetupActions()
        {
            if (!_hasReachedPointOfNoReturn)
            {
                
            }
        }

        public void ProcessCurse()
        {
            if (_hasReachedPointOfNoReturn)
            {
                BuildingOwner = TeamOwner.Enemy;
                return;
            }

            switch (BuildingOwner)
            {
                case TeamOwner.Neutral:
                    IncreaseCurse();
                    break;
                case TeamOwner.Player:
                    DecreaseCurse();
                    break;
            }

            UpdateCurseState();
        }

        private void IncreaseCurse()
        {
            CurrentCurseValue = Mathf.Min(CurrentCurseValue + CURSE_CHANGE_RATE, MAX_CURSE_VALUE);
            
            if (CurrentCurseValue >= CURSE_THRESHOLD_POINT_OF_NO_RETURN)
            {
                _hasReachedPointOfNoReturn = true;
                BuildingOwner = TeamOwner.Enemy;
                OnPermanentlyCorrupted();
            }
        }

        private void DecreaseCurse()
        {
            if (_hasReachedPointOfNoReturn) return;
            CurrentCurseValue = Mathf.Max(CurrentCurseValue - CURSE_CHANGE_RATE, 0f);
        }

        private void UpdateCurseState()
        {
            float cursePercentage = CurrentCurseValue / MAX_CURSE_VALUE;
            
            if (_meshRenderer != null)
            {
                Color baseColor = Color.white;
                Color cursedColor = Color.red;
                _meshRenderer.material.color = Color.Lerp(baseColor, cursedColor, cursePercentage);
            }
        }

        private void OnPermanentlyCorrupted()
        {
            _availableActions.Clear();
            if (_meshRenderer != null)
            {
                _meshRenderer.material.color = Color.black;
            }
        }

        public void OnCaptured(TeamOwner newOwner)
        {
            if (_hasReachedPointOfNoReturn && newOwner == TeamOwner.Player)
            {
                Debug.Log("This settlement is beyond saving!");
                return;
            }

            BuildingOwner = newOwner;
            
            if (newOwner == TeamOwner.Player)
            {
                SetupActions();
            }
        }
    }
}