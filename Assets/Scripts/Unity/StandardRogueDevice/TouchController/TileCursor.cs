using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class TileCursor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Sprite[] _sprites = null;

        private int animationTime;

        private void Awake()
        {
            animationTime = 0;
        }

        public void UpdateAnimation(bool pointing, Vector2 point, int deltaTime)
        {
            if (pointing)
            {
                animationTime += deltaTime;
                var spriteIndex = animationTime / 8 % _sprites.Length;
                _spriteRenderer.sprite = _sprites[spriteIndex];
                transform.localPosition = new Vector3(point.x, point.y, -9f);
                _spriteRenderer.enabled = true;
            }
            else
            {
                _spriteRenderer.enabled = false;
            }
        }
    }
}
