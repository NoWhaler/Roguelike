using Core.TurnBasedSystem;
using Game.UI.UIGameplayScene.Settings;
using Game.UI.UIGameplayScene.TechnologyPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.UIGameplayScene.BuildingsActionPanel
{
    public class GameActionsPanel: MonoBehaviour
    {
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private Button _openTechnologiesButton;
        [SerializeField] private Button _openSettingsButton;
        
        [SerializeField] private TMP_Text _dayText;

        private GameTurnController _gameTurnController;
        private UITechnologyPanel _technologyPanel;
        private UISettingsPanel _settingsPanel;

        [Inject]
        public void Construct(GameTurnController gameTurnController, UITechnologyPanel technologyPanel, UISettingsPanel settingsPanel)
        {
            _gameTurnController = gameTurnController;
            _technologyPanel = technologyPanel;
            _settingsPanel = settingsPanel;
        }

        private void OnEnable()
        {
            _endTurnButton.onClick.AddListener(EndTurn);
            _openTechnologiesButton.onClick.AddListener(OpenTechPanel);
            _openSettingsButton.onClick.AddListener(OpenSettingsPanel);
            
            _gameTurnController.OnTurnChanged += UpdateDayText;
        }

        private void OnDisable()
        {
            _endTurnButton.onClick.RemoveListener(EndTurn);
            _openTechnologiesButton.onClick.RemoveListener(OpenTechPanel);
            _openSettingsButton.onClick.RemoveListener(OpenSettingsPanel);
            
            _gameTurnController.OnTurnChanged -= UpdateDayText;
        }

        private void EndTurn()
        {
            _gameTurnController.EndTurn();
        }

        private void UpdateDayText(int day)
        {
            _dayText.text = $"Day: {day}";
        }

        private void OpenTechPanel()
        {
            _technologyPanel.ShowPanel();
        }

        private void OpenSettingsPanel()
        {
            _settingsPanel.ShowPanel();
        }
    }
}