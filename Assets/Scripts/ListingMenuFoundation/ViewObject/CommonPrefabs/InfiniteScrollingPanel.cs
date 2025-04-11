using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Infinite Scrolling Panel")]
    public class InfiniteScrollingPanel : MonoBehaviour
    {
        [SerializeField] private Vector2 _scrollVelocity = new(-64f, 64f);
        [SerializeField] private float _repeatTime = 1f;

        private Vector2 startPosition;
        private float elapsedTime;

        private void Start()
        {
            startPosition = transform.localPosition;
            elapsedTime = 0f;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= _repeatTime)
            {
                elapsedTime -= _repeatTime;
            }

            transform.localPosition = startPosition + _scrollVelocity * elapsedTime;
        }
    }
}
