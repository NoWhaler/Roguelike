using System;
using Game.Buildings.BuildingsType;
using Game.Units;
using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;

namespace Game.WorldGeneration.Hex
{
    public class HexModel: MonoBehaviour
    {
        [field: SerializeField] public Vector3 HexPosition { get; set; }
        
        [field: SerializeField] public int Q { get; set; }
        [field: SerializeField] public int R { get; set; }
        [field: SerializeField] public int S { get; set; }

        [field: SerializeField] public bool IsVisible { get; private set; } = true;
        
        [field: SerializeField] public BiomeType BiomeType { get; set; }
        
        [SerializeField] private SpriteRenderer _hexOutlineImage;

        [SerializeField] private SpriteRenderer _unitRangeHighlight;

        [SerializeField] private SpriteRenderer _unitPathHighlight;

        [SerializeField] private SpriteRenderer _fogRenderer;   
        
        [field: SerializeField] public int HexIndex { get; set; }
        
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

        public void SetFog(bool isEnabled)
        {
            IsVisible = !isEnabled;
            _fogRenderer.enabled = isEnabled;
        }

        public void SetUnitRangeHighlight(bool isHighlighted)
        {
            _unitRangeHighlight.enabled = isHighlighted;
        }

        public void SetUnitPathHighlight(bool isHighlighted)
        {
            if (IsVisible)
            {
                _unitPathHighlight.enabled = isHighlighted;
            }
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