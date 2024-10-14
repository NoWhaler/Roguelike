using Game.Buildings.BuildingsType;
using Game.Units;
using UnityEngine;

namespace Game.WorldGeneration.Hex
{
    public struct HexCoordinate
    {
        public int Q, R, S; 
        
        public HexCoordinate(int q, int r)
        {
            Q = q;
            R = r;
            S = -q - r;
        }
    }
    
    public class HexModel: MonoBehaviour
    {
        [field: SerializeField] public Vector3 HexPosition { get; set; }
        
        [field: SerializeField] public int Q { get; set; }
        [field: SerializeField] public int R { get; set; }
        [field: SerializeField] public int S { get; set; }

        [SerializeField] private SpriteRenderer _hexOutlineImage;
        
        [field: SerializeField] public Building CurrentBuilding { get; set; }

        [field: SerializeField] public Unit CurrentUnit { get; set; }
        
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

        public bool IsHexEmpty()
        {
            return CurrentBuilding == null && CurrentUnit == null;
        }

        public void SetBuilding(ref Building building)
        {
            CurrentBuilding = building;
            CurrentBuilding.Initialize();
        }

        public void SetUnit(ref Unit unit)
        {
            CurrentUnit = unit;
            CurrentUnit.Initialize();
        }
    }
}