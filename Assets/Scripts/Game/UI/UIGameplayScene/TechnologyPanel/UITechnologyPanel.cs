using System.Collections.Generic;
using Game.Technology.Controller;
using Game.Technology.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.UIGameplayScene.TechnologyPanel
{
    public class UITechnologyPanel: MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private Button _economyButton;

        [SerializeField] private Button _militaryButton;

        [SerializeField] private Button _buildingsButton;

        [SerializeField] private GameObject _content;

        [SerializeField] private TechnologyView _technologyViewPrefab;
        
        private readonly List<TechnologyView> _activeTechnologies = new();

        private TechnologiesController _technologiesController;

        private DiContainer _diContainer;

        [Inject]
        private void Constructor(TechnologiesController technologiesController, DiContainer diContainer)
        {
            _technologiesController = technologiesController;
            _diContainer = diContainer;
        }

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(HidePanel);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(HidePanel);
        }

        public void ShowTechnologies()
        {
            foreach (var tech in _technologiesController.GetAllTechnologies().Values)
            {
                var techView = _diContainer.InstantiatePrefabForComponent<TechnologyView>(_technologyViewPrefab, _content.transform);
                techView.SetTechnology(tech);
                
                _activeTechnologies.Add(techView);
            }
        }

        public void UpdateTechViews()
        {
            foreach (var technology in _activeTechnologies)
            {
                technology.UpdateView();
            }
        }

        public void ShowPanel()
        {
            gameObject.SetActive(true);
        }

        private void HidePanel()
        {
            foreach (var tech in _activeTechnologies)
            {
                Destroy(tech.gameObject);
            }
            
            _activeTechnologies.Clear();
            
            gameObject.SetActive(false);
        }
    }
}