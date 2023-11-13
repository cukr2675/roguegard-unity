using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace RoguegardUnity
{
    /// <summary>
    /// メニューウィンドウの背景
    /// </summary>
    public class MenuWindowBackground : MonoBehaviour
    {
        [SerializeField] private Image _imageA = null;
        public Image ImageA => _imageA;

        [SerializeField] private Image _imageB = null;
        public Image ImageB => _imageB;
    }
}
