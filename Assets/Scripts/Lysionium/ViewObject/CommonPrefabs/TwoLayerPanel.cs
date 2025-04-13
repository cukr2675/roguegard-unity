using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Lysionium
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
