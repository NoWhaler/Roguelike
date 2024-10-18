using System.Collections.Generic;
using Game.Buildings;
using Game.Buildings.BuildingsType;
using Game.Buildings.Interfaces;
using Game.Buildings.View;
using Game.Units.View;
using UnityEngine;
using Zenject;

namespace Game.UI.UIGameplayScene.BuildingsActionPanel
{
    public class UIBuildingsActionPanel: MonoBehaviour
    {
        [SerializeField] private BuildingActionView _actionViewPrefab;

        [SerializeField] private Transform _panelTransform;
        
        private List<BuildingActionView> _actionViews = new List<BuildingActionView>();
        
        [SerializeField] private List<UnitSelectorView> _unitViews = new List<UnitSelectorView>();

        private DiContainer _diContainer;
        
        [Inject]
        private void Constructor(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void ShowActions(List<IBuildingAction> actions)
        {
            ClearActions();

            foreach (var action in actions)
            {
                var actionView = _diContainer.InstantiatePrefabForComponent<BuildingActionView>(_actionViewPrefab, _panelTransform);
                actionView.SetAction(action);
                _actionViews.Add(actionView);
            }

            gameObject.SetActive(true);
        }

        public void SetUnitCount(ref Building currentBuilding)
        {
            foreach (var unitView in _unitViews)
            {
                unitView.UpdateUnitCount(ref currentBuilding);
            }
        }

        public void UpdateActionViews()
        {
            ClearActions();
            
            foreach (var actionView in _actionViews)
            {
                actionView.UpdateView();
            }
        }
        
        public void Hide()
        {
            ClearActions();
            gameObject.SetActive(false);
        }

        private void ClearActions()
        {
            foreach (var actionView in _actionViews)
            {
                Destroy(actionView.gameObject);
            }
            _actionViews.Clear();
        }
    }
}