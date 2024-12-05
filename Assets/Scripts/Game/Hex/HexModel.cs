using Game.Buildings.BuildingsType;
using Game.ProductionResources;
using Game.Units;
using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;

namespace Game.Hex
{
    public class HexModel: MonoBehaviour
    {
        [field: SerializeField] public Vector3 HexPosition { get; set; }
        
        [field: SerializeField] public int Q { get; set; }
        [field: SerializeField] public int R { get; set; }
        [field: SerializeField] public int S { get; set; }

        [field: SerializeField] public bool IsVisible { get; private set; }
        
        [field: SerializeField] public BiomeType BiomeType { get; set; }
        
        [field: SerializeField] public int BiomeIndex { get; set; } = -1;
        
        [SerializeField] private SpriteRenderer _hexOutlineImage;

        [SerializeField] private SpriteRenderer _unitRangeHighlight;

        [SerializeField] private SpriteRenderer _unitPathHighlight;

        [SerializeField] private SpriteRenderer _fogRenderer;   
        
        [field: SerializeField] public Building CurrentBuilding { get; set; }

        [field: SerializeField] public Unit CurrentUnit { get; set; }
        
        [field: SerializeField] public ResourceDeposit ResourceDeposit { get; set; }
        
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
            
            UpdateEntitiesVisibility(!isEnabled);
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
        
        private void UpdateEntitiesVisibility(bool isVisible)
        {
            if (CurrentBuilding != null)
            {
                CurrentBuilding.UpdateMeshVisibility(isVisible);
            }

            if (CurrentUnit != null)
            {
                CurrentUnit.UpdateMeshVisibility(isVisible);
            }

            if (ResourceDeposit != null)
            {
                ResourceDeposit.UpdateMeshVisibility(isVisible);
            }
        }
    }
}