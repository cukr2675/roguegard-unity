using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Two Layer Panel")]
    public class TwoLayerPanel : MonoBehaviour
    {
        [SerializeField] private Image _background = null;
        public Image Background => _background;

        [SerializeField] private Image _foreground = null;
        public Image Foreground => _foreground;
    }
}
