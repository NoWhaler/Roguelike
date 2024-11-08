using Core.TurnBasedSystem;
using Game.Technology.Controller;
using Game.Technology.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Technology.View
{
    public class TechnologyView: MonoBehaviour
    {
        [SerializeField] private TMP_Text _turnsLeftText;

        [SerializeField] private Button _technologyButton;

        [SerializeField] private Image _checkMark;

        private GameTurnController _gameTurnController;

        private TechnologiesController _technologiesController;

        private TechnologyModel _technology;

        [Inject]
        private void Constructor(GameTurnController gameTurnController, TechnologiesController technologiesController)
        {
            _gameTurnController = gameTurnController;
            _technologiesController = technologiesController;
        }
        
        private void OnEnable()
        {
            _technologyButton.onClick.AddListener(StartTechnology);
            _gameTurnController.OnTurnEnded += UpdateView;
        }

        private void OnDisable()
        {
            _technologyButton.onClick.RemoveListener(StartTechnology);
            _gameTurnController.OnTurnEnded -= UpdateView;
        }

        public void SetTechnology(TechnologyModel technologyModel)
        {
            _technology = technologyModel;
            UpdateView();   
        }
        
        public void UpdateView()
        {
            _turnsLeftText.text = _technology.TurnsLeft.ToString();
            _technologyButton.interactable = !_technology.IsResearched;
            _checkMark.enabled = _technology.IsResearched;
        }

        private void StartTechnology()
        {
            _gameTurnController.AddActiveTechnology(_technology);
            _technologiesController.StartTechnology(_technology);
        }

        private void CancelTechnology()
        {
            
        }
    }
}