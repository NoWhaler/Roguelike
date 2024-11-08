using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.UIGameplayScene.TechnologyPanel
{
    public class UITechnologyPanel: MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private Button _economyButton;

        [SerializeField] private Button _militaryButton;

        [SerializeField] private Button _buildingsButton;

        [SerializeField] private GameObject _economyTechnologyContent;
        
        [SerializeField] private GameObject _militaryTechnologyContent;
        
        [SerializeField] private GameObject _buildingsTechnologyContent;
    }
}