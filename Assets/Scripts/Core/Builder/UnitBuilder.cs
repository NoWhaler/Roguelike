using Game.Hex;
using Game.Units;
using Game.Units.Enum;
using ScriptableObjects;
using UnityEngine;

namespace Core.Builder
{
    public class UnitBuilder
    {
        private readonly Unit _unit;
        private UnitConfigSO _config;

        public UnitBuilder(Unit unit, UnitConfigSO config)
        {
            _unit = unit;
            _config = config;
        }

        public UnitBuilder WithType()
        {
            _unit.UnitType = _config.UnitType;
            return this;
        }

        public UnitBuilder WithTeam(UnitTeamType teamType)
        {
            _unit.UnitTeamType = teamType;
            return this;
        }

        public UnitBuilder WithHealth()
        {
            _unit.MaxHealth = _config.MaxHealth;
            _unit.CurrentHealth = _config.MaxHealth;
            return this;
        }

        public UnitBuilder WithMovementPoints()
        {
            _unit.MaxMovementPoints = _config.MaxMovementPoints;
            _unit.CurrentMovementPoints = _config.MaxMovementPoints;
            return this;
        }

        public UnitBuilder WithDamage()
        {
            _unit.MinDamage = _config.MinDamage;
            _unit.MaxDamage = _config.MaxDamage;
            return this;
        }

        public UnitBuilder WithAttackRange()
        {
            _unit.AttackRange = _config.AttackRange;
            return this;
        }

        public UnitBuilder AtPosition(HexModel hex)
        {
            _unit.CurrentHex = hex;
            hex.CurrentUnit = _unit;
            _unit.gameObject.SetActive(true);
            _unit.transform.position = new Vector3(hex.HexPosition.x, hex.HexPosition.y + 5f, hex.HexPosition.z);
            return this;
        }

        public Unit Build()
        {
            _unit.Initialize();
            return _unit;
        }
    }
}