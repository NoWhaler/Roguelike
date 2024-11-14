namespace Game.Technology
{
    public enum TechnologyEffectType
    {
        UnitDamage,
        UnitMovement,
        ResourceProduction,
        UnitProduction,
        UnitHealth
    }
    
    public class TechnologyEffect
    {
        public TechnologyEffectType EffectType { get; set; }
        public float Value { get; set; }    
    }
}