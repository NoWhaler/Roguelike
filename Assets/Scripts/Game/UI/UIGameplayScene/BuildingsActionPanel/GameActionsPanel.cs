using Core.TurnBasedSystem;
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
        
        [SerializeField] private TMP_Text _dayText;

        private GameTurnController _gameTurnController;
        private UITechnologyPanel _technologyPanel;

        [Inject]
        public void Construct(GameTurnController gameTurnController, UITechnologyPanel technologyPanel)
        {
            _gameTurnController = gameTurnController;
            _technologyPanel = technologyPanel;
        }

        private void OnEnable()
        {
            _endTurnButton.onClick.AddListener(EndTurn);
            _openTechnologiesButton.onClick.AddListener(OpenTechPanel);
            _gameTurnController.OnTurnChanged += UpdateDayText;
        }

        private void OnDisable()
        {
            _endTurnButton.onClick.RemoveListener(EndTurn);
            _openTechnologiesButton.onClick.RemoveListener(OpenTechPanel);
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
            _technologyPanel.ShowTechnologies();
        }
    }
}