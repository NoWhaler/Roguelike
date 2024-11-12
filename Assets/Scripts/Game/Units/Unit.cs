using Game.Hex;
using Game.Units.Enum;
using UnityEngine;

namespace Game.Units
{
    public abstract class Unit: MonoBehaviour
    {
        [field: SerializeField] public UnitType UnitType { get; set; }
        
        [field: SerializeField] public UnitTeamType UnitTeamType { get; set; }
        
        [field: SerializeField] public float MaxHealth { get; set; }
        
        [field: SerializeField] public float CurrentHealth { get; set; }
        
        [field: SerializeField] public int MaxMovementPoints { get; set; }
        
        [field: SerializeField] public int CurrentMovementPoints { get; private set; }
        
        public HexModel CurrentHex { get; set; }

        public void Initialize()
        {
            CurrentHealth = MaxHealth;
            CurrentMovementPoints = MaxMovementPoints;
        }
        
        public void ResetMovementPoints()
        {
            CurrentMovementPoints = MaxMovementPoints;
        }

        private bool CanMove(int distance)
        {
            return CurrentMovementPoints >= distance;
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
            Destroy(gameObject);
        }
    }
}