using System;
using System.Collections.Generic;

namespace Game.Hex
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
        
        public List<HexModel> GetHexesInRadius(HexModel centerHex, int radius)
        {
            List<HexModel> hexesInRadius = new List<HexModel>();
        
            for (int dq = -radius; dq <= radius; dq++)
            {
                for (int dr = Math.Max(-radius, -dq - radius); dr <= Math.Min(radius, -dq + radius); dr++)
                {
                    int ds = -dq - dr;
                    HexModel hex = GetHexAt(centerHex.Q + dq, centerHex.R + dr, centerHex.S + ds);
                    if (hex != null)
                    {
                        hexesInRadius.Add(hex);
                    }
                }
            }
        
            return hexesInRadius;
        }
        
        public List<HexModel> GetNeighbors(HexModel hex)
        {
            List<HexModel> neighbors = new List<HexModel>();
            int[][] directions = new int[][]
            {
                new int[] {+1, -1, 0}, new int[] {+1, 0, -1}, new int[] {0, +1, -1},
                new int[] {-1, +1, 0}, new int[] {-1, 0, +1}, new int[] {0, -1, +1}
            };

            foreach (int[] dir in directions)
            {
                int neighborQ = hex.Q + dir[0];
                int neighborR = hex.R + dir[1];
                int neighborS = hex.S + dir[2];

                HexModel neighbor = GetHexAt(neighborQ, neighborR, neighborS);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
    }
}