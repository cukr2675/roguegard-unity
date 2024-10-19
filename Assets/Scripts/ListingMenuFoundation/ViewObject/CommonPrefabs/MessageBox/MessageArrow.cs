using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// <see cref="MessageBox.NextCharacterPosition"/> に追従するコンポーネント
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Message Arrow")]
    public class MessageArrow : MonoBehaviour
    {
        [SerializeField] private MessageBox _target = null;
        [SerializeField] private int _deltaCharacterIndex = 0;
        [SerializeField] private float _normalizedX = .5f;

        private void Update()
        {
            transform.localPosition = _target.GetCurrentCharacterPosition(_deltaCharacterIndex, _normalizedX);
        }
    }
}
