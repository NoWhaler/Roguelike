using Game.Hex;
using Game.Units.Controller;
using Game.Units.Enum;
using Game.Units.View;
using ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Game.Units
{
    public abstract class Unit: MonoBehaviour
    {
        [field: SerializeField] public UnitType UnitType { get; set; }
        
        [field: SerializeField] public UnitTeamType UnitTeamType { get; set; }
        
        [SerializeField] private UnitView _unitView;

        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }
        
        [field: SerializeField] public int MaxMovementPoints { get; set; }
        
        [field: SerializeField] public int CurrentMovementPoints { get; set; }
        
        [field: SerializeField] public float MinDamage { get; set; }
        
        [field: SerializeField] public float MaxDamage { get; set; }
        
        [field: SerializeField] public int AttackRange { get; set; }
        
        [field: SerializeField] public bool HasAttackedThisTurn { get; private set; }
        
        public HexModel CurrentHex { get; set; }

        [Inject] private UnitsController _unitsController;

        public void Initialize()
        {
            CurrentHealth = MaxHealth;
            CurrentMovementPoints = MaxMovementPoints;
            HasAttackedThisTurn = false;
            
            _unitView.Initialize(UnitTeamType);
            _unitView.UpdateHealthBar(CurrentHealth, MaxHealth);
        }
        
        public void ResetMovementPoints()
        {
            CurrentMovementPoints = MaxMovementPoints;
            HasAttackedThisTurn = false;
        }
        
        public void SetMovementPointsToZero()
        {
            CurrentMovementPoints = 0;
        }

        private bool CanMove(int distance)
        {
            return CurrentMovementPoints >= distance;
        }
        
        public float Attack()
        {
            if (HasAttackedThisTurn) return 0;
            
            var damage = Mathf.RoundToInt(Random.Range(MinDamage, MaxDamage));
            
            HasAttackedThisTurn = true;
            return damage;
        }
        
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            _unitView.UpdateHealthBar(CurrentHealth, MaxHealth);
            if (CurrentHealth <= 0)
            {
                DisableUnit();
            }
        }
        
        public void Move(HexModel newHex, int movementCost)
        {
            if (CanMove(movementCost))
            {
                CurrentHex.CurrentUnit = null;
                CurrentHex = newHex;
                newHex.CurrentUnit = this;
                transform.position = new Vector3(newHex.HexPosition.x, newHex.HexPosition.y + 5f, newHex.HexPosition.z);
                CurrentMovementPoints -= movementCost;
            }
        }

        public void DisableUnit()
        {
            _unitsController.ReturnUnitToPool(this);
        }
    }
}