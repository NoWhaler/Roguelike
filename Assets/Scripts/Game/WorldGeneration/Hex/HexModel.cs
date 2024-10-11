using System;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private Image _hexOutlineImage;
        
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
    }
}