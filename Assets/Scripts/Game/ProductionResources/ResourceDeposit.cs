using Game.ProductionResources.Enum;
using UnityEngine;

namespace Game.ProductionResources
{
    public class ResourceDeposit: MonoBehaviour
    {
        [field: SerializeField] public ResourceType ResourceType { get; set; }
        
        [SerializeField] private MeshRenderer _meshRenderer;
        
        public void UpdateMeshVisibility(bool isVisible)
        {
            _meshRenderer.enabled = isVisible;
        }
    }
}