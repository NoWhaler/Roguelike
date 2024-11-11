using UnityEngine;

namespace Game.EditorSettings.Model
{
    public class SettingsModel: MonoBehaviour
    {
        [field: SerializeField] public bool IsFogEnabled { get; set; }
    }
}