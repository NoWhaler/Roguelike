using Game.Units.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Units.View
{
    public class UnitView: MonoBehaviour
    {
        [SerializeField] private Image _unitIndicator;
        
        private Color _enemyColor = Color.red;
        
        private Color _playerColor = Color.green;
        
        public void Initialize(UnitTeamType teamType)
        {
            if (_unitIndicator != null)
            {
                _unitIndicator.color = teamType == UnitTeamType.Player ? _playerColor : _enemyColor;
            }
        }
    }
}