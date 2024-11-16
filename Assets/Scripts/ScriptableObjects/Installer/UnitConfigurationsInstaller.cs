using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace ScriptableObjects.Installer
{
    public class UnitConfigurationsInstaller: MonoInstaller
    {
        [SerializeField] private UnitConfigSO _archerConfig;
        [SerializeField] private UnitConfigSO _swordsmanConfig;
        [SerializeField] private UnitConfigSO _crossbowmanConfig;
        [SerializeField] private UnitConfigSO _horsemanConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<UnitConfigSO>().WithId(UnitType.Archer).FromInstance(_archerConfig);
            Container.Bind<UnitConfigSO>().WithId(UnitType.Swordsman).FromInstance(_swordsmanConfig);
            Container.Bind<UnitConfigSO>().WithId(UnitType.Crossbowman).FromInstance(_crossbowmanConfig);
            Container.Bind<UnitConfigSO>().WithId(UnitType.Horseman).FromInstance(_horsemanConfig);
        }
    }
}