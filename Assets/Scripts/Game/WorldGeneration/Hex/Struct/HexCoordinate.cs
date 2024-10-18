namespace Game.WorldGeneration.Hex.Struct
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
}