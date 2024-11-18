using Game.Units.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Units.View
{
    public class UnitView: MonoBehaviour
    {
        [SerializeField] private Image _unitIndicator;
        [SerializeField] private Image _healthBarBackground;
        [SerializeField] private Image _healthBarFill;
        
        private Color _enemyColor = Color.red;
        
        private Color _playerColor = Color.green;
        
        public void Initialize(TeamOwner teamOwner)
        {
            if (_unitIndicator != null)
            {
                _unitIndicator.color = teamOwner == TeamOwner.Player ? _playerColor : _enemyColor;
            }
        }
        
        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
            _healthBarFill.fillAmount = healthPercentage;
        }
    }
}