using System.Collections.Generic;

namespace Game.WorldGeneration.Hex
{
    public class HexGridController
    {
        private Dictionary<(int, int, int), HexModel> _hexGrid = new Dictionary<(int, int, int), HexModel>();

        public Dictionary<(int, int, int), HexModel> GetAllHexes()
        {
            return _hexGrid;
        }

        public void SetHex(HexModel hex)
        {
            _hexGrid.Add((hex.Q, hex.R, hex.S), hex);
        }
    
        public HexModel GetHexAt(int q, int r, int s)
        {
            if (_hexGrid.TryGetValue((q, r, s), out HexModel hex))
            {
                return hex;
            }
            return null;
        }   
    }
}