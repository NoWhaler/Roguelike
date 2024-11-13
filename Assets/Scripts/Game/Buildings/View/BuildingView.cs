using System.Linq;
using Game.Buildings.BuildingsType;
using Game.ProductionResources.Controller;
using Game.UI.UIGameplayScene.SelectionHandling;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Buildings.View
{
    public class BuildingView : MonoBehaviour
    {
        [field: SerializeField] private Image _iconImage;

        [field: SerializeField] private Button _imageButton;

        [field: SerializeField] private Building _selectedBuilding;

        private UISelectionHandler _uiSelectionHandler;

        private ResourcesController _resourcesController;

        [Inject]
        private void Constructor(UISelectionHandler uiSelectionHandler, ResourcesController resourcesController)
        {
            _uiSelectionHandler = uiSelectionHandler;
            _resourcesController = resourcesController;
        }

        private void OnEnable()
        {
            _imageButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _imageButton.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (HasEnoughResources())
            {
                _uiSelectionHandler.SelectUIBuilding(_selectedBuilding);
            }
            else
            {
                Debug.Log("Not enough resources to build this building!");
            }
        }
        
        private bool HasEnoughResources()
        {
            var buildingCost = _selectedBuilding.GetBuildingCost();
            if (buildingCost == null) return true;

            return buildingCost.ResourceCosts.All(resourceCost =>
                _resourcesController.GetResourceAmount(resourceCost.ResourceType) >= resourceCost.Amount);
        }
    }
}