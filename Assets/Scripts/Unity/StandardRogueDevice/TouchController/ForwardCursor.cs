using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    public class ForwardCursor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null;

        [SerializeField] private Sprite _sprite = null;
        [SerializeField] private Sprite _spriteDiagonal = null;

        public void UpdateAnimation(bool visiblePlayer, Vector3 playerPosition, RogueDirection playerDirection)
        {
            if (visiblePlayer)
            {
                var angleIndex = (int)playerDirection;
                _spriteRenderer.sprite = angleIndex % 2 == 0 ? _sprite : _spriteDiagonal;
                var eulerZ = (angleIndex / 2) * 90f;

                _spriteRenderer.enabled = true;
                transform.localRotation = Quaternion.Euler(0f, 0f, eulerZ);
                var forward = new Vector3(playerDirection.Forward.x, playerDirection.Forward.y) / 2f;
                forward.y += .25f;
                transform.localPosition = new Vector3(playerPosition.x, playerPosition.y, -8f) + forward;
            }
            else
            {
                _spriteRenderer.enabled = false;
            }
        }
    }
}
