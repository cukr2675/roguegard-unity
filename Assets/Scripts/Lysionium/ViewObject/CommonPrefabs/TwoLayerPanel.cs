using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/LUI Two Layer Panel")]
    public class TwoLayerPanel : MonoBehaviour
    {
        [SerializeField] private Image _background = null;
        public Image Background => _background;

        [SerializeField] private Image _foreground = null;
        public Image Foreground => _foreground;
    }
}
