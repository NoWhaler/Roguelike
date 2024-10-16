using System.Collections.Generic;
using Game.ProductionResources.View;
using UnityEngine;

namespace Game.UI.UIGameplayScene.UIResourcesPanel
{
    public class ResourcesPanel: MonoBehaviour
    {
        [SerializeField] private List<ResourceView> _resourceViews;
        
        public void UpdateResourcesAmount()
        {
            foreach (var resourceView in _resourceViews)
            {
                resourceView.UpdateResourceCount();
            }
        }
    }
}