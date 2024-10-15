using Core.TurnBasedSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.UIGameplayScene.BuildingsActionPanel
{
    public class GameActionsPanel: MonoBehaviour
    {
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private TMP_Text _dayText;

        private GameTurnController _gameTurnController;

        [Inject]
        public void Construct(GameTurnController gameTurnController)
        {
            _gameTurnController = gameTurnController;
        }

        private void OnEnable()
        {
            _endTurnButton.onClick.AddListener(EndTurn);
            _gameTurnController.OnTurnChanged += UpdateDayText;
        }

        private void OnDisable()
        {
            _endTurnButton.onClick.RemoveListener(EndTurn);
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
    }
}