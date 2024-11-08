using System;

namespace Game.Technology.Model
{
    [Serializable]
    public class TechnologyModel
    {
        public string Name { get; private set; }
        
        public string Description { get; private set; }
        
        public bool IsResearched { get; private set; }
        
        public int TurnsRequired { get; private set; }
        
        public int TurnsLeft { get; private set; }
    }
}