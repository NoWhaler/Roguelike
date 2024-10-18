using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Units.View
{
    public class UnitView: MonoBehaviour
    {
        [SerializeField] private Image _healthBar;

        [SerializeField] private Image _movementImage;

        [SerializeField] private TMP_Text _healthAmountText;

        [SerializeField] private TMP_Text _movementAmountText;
    }
}