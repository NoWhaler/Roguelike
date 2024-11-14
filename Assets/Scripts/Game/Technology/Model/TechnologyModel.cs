using System;
using System.Collections.Generic;

namespace Game.Technology.Model
{
    [Serializable]
    public class TechnologyModel
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public bool IsResearched { get; set; }
        
        public int TurnsRequired { get; set; }
        
        public int TurnsLeft { get; set; }
        
        public List<TechnologyEffect> Effects { get; set; } = new List<TechnologyEffect>();
    }
}