using Zenject;

namespace Game.Characters.Player.Stats.Models
{
    public class PlayerStatsModel: IInitializable
    {
        public float Health { get; set; }

        public float Damage { get; set; }
        
        public void Initialize()
        {
            Health = 50;
            Damage = 5;
        }
    }
}