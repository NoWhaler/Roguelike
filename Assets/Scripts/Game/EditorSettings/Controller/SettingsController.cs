using Game.WorldGeneration.Hex;
using UnityEngine;
using Zenject;

namespace Game.EditorSettings.Controller
{
    public class SettingsController
    {
        private HexGridController _hexGridController;
        
        private const string GRID_ENABLED_PARAM = "_GlobalGridEnabled";
        
        [Inject]
        private void Constructor(HexGridController hexGridController)
        {
            _hexGridController = hexGridController;
        }

        public void EnableDisableFog(bool value)
        {
            var allHexes = _hexGridController.GetAllHexes();
            foreach (var hex in allHexes.Values)
            {
                hex.SetFog(value);
            }
        }

        public void EnableDisableGrid(bool value)
        {
            Shader.SetGlobalFloat(GRID_ENABLED_PARAM, value ? 1.0f : 0.0f);
        }

        public string GetGridParameter()
        {
            return GRID_ENABLED_PARAM;
        }
    }
}