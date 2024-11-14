using System.Collections.Generic;
using Game.Technology;
using Game.Technology.Controller;
using Game.Technology.Model;
using Zenject;

namespace Core.Lifetime.Initialization
{
    public class TechnologiesInitialization: IInitializable
    {
        private TechnologiesController _technologiesController;

        private readonly Dictionary<string, TechnologyModel> _allTechnologies = new()
        {
            {
                "DamageBoost", new TechnologyModel
                {
                    Name = "Advanced Weaponry",
                    Description = "Increases all units' damage by 1",
                    IsResearched = false,
                    TurnsRequired = 2,
                    TurnsLeft = 2,
                    Effects = new List<TechnologyEffect>
                    {
                        new() { EffectType = TechnologyEffectType.UnitDamage, Value = 1f }
                    }
                }
            },
            {
                "MovementBoost", new TechnologyModel
                {
                    Name = "Advanced Tactics",
                    Description = "Increases all units' movement by 1",
                    IsResearched = false,
                    TurnsRequired = 2,
                    TurnsLeft = 2,
                    Effects = new List<TechnologyEffect>
                    {
                        new() { EffectType = TechnologyEffectType.UnitMovement, Value = 1f }
                    }
                }
            },
            {
                "ResourceBoost", new TechnologyModel
                {
                    Name = "Resource Extraction",
                    Description = "Increases resource production by 1",
                    IsResearched = false,
                    TurnsRequired = 2,
                    TurnsLeft = 2,
                    Effects = new List<TechnologyEffect>
                    {
                        new() { EffectType = TechnologyEffectType.ResourceProduction, Value = 1f }
                    }
                }
            },
            {
                "ProductionBoost", new TechnologyModel
                {
                    Name = "Mass Production",
                    Description = "Reduces unit production time by 1 turn",
                    IsResearched = false,
                    TurnsRequired = 2,
                    TurnsLeft = 2,
                    Effects = new List<TechnologyEffect>
                    {
                        new() { EffectType = TechnologyEffectType.UnitProduction, Value = 1f }
                    }
                }
            },
            {
                "HealthBoost", new TechnologyModel
                {
                    Name = "Advanced Medicine",
                    Description = "Increases all units' HP by 10",
                    IsResearched = false,
                    TurnsRequired = 2,
                    TurnsLeft = 2,
                    Effects = new List<TechnologyEffect>
                    {
                        new() { EffectType = TechnologyEffectType.UnitHealth, Value = 10f }
                    }
                }
            }
        };
        
        [Inject]
        private void Constructor(TechnologiesController technologiesController)
        {
            _technologiesController = technologiesController;
        }
        
        public void Initialize()
        {
            _technologiesController.InitializeResearch(_allTechnologies);
        }
    }
}