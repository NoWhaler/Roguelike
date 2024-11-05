using System.Collections.Generic;
using Game.Buildings.BuildingsType;
using Game.Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.WorldGeneration.Hex
{
    
    public class HexModel: MonoBehaviour
    {
        [field: SerializeField] public Vector3 HexPosition { get; set; }
        
        [field: SerializeField] public int Q { get; set; }
        [field: SerializeField] public int R { get; set; }
        [field: SerializeField] public int S { get; set; }
        
        [SerializeField] private SpriteRenderer _hexOutlineImage;

        [SerializeField] private SpriteRenderer _unitRangeHighlight;

        [SerializeField] private SpriteRenderer _unitPathHighlight;
        
        public Building CurrentBuilding { get; set; }

        public Unit CurrentUnit { get; set; }
        
        public void SetLogicalCoordinates(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
        }

        public void SetOutline()
        {
            _hexOutlineImage.enabled = !_hexOutlineImage.enabled;
        }

        public void SetUnitRangeHighlight(bool isHighlighted)
        {
            _unitRangeHighlight.enabled = isHighlighted;
        }

        public void SetUnitPathHighlight(bool isHighlighted)
        {
            _unitPathHighlight.enabled = isHighlighted;
        }
        
        public bool IsHexEmpty()
        {
            return CurrentBuilding == null && CurrentUnit == null;
        }

        public void SetBuilding(ref Building building)
        {
            CurrentBuilding = building;
            CurrentBuilding.Initialize(this);
        }

        public void SetUnit(ref Unit unit)
        {
            CurrentUnit = unit;
            CurrentUnit.Initialize();
        }
    }
}