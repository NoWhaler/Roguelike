using Game.ProductionResources.Controller;
using Game.ProductionResources.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.ProductionResources.View
{
    public class ResourceView: MonoBehaviour
    {
        [SerializeField] private TMP_Text _resourceCount;

        [SerializeField] private Image _resourceIcon;

        [SerializeField] private ResourceType _resourceType;

        private ResourcesController _resourcesController;
        
        [Inject]
        private void Constructor(ResourcesController resourcesController)
        {
            _resourcesController = resourcesController;
        }
        
        public void UpdateResourceCount()
        {
            int count = _resourcesController.GetResourceAmount(_resourceType);
            _resourceCount.text = count.ToString();
        }
    }
}