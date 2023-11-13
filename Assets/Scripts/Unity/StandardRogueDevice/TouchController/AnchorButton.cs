using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using Roguegard;

namespace RoguegardUnity
{
    public class AnchorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _image = null;
        [SerializeField] private AnchorBalloon _balloon = null;

        [Header("Keys")]

        [SerializeField] private CanvasGroup _keysCanvasGroup = null;

        [SerializeField] private AnchorKeyItem _upperLeftKey = null;
        [SerializeField] private AnchorKeyItem _upKey = null;
        [SerializeField] private AnchorKeyItem _upperRightKey = null;
        [SerializeField] private AnchorKeyItem _leftKey = null;
        [SerializeField] private AnchorKeyCenter _centerKey = null;
        [SerializeField] private AnchorKeyItem _rightKey = null;
        [SerializeField] private AnchorKeyItem _lowerLeftKey = null;
        [SerializeField] private AnchorKeyItem _downKey = null;
        [SerializeField] private AnchorKeyItem _lowerRightKey = null;

        /// <summary>
        /// ほかのキーを触らず、指定のキーを押して離したとき true を取得する。
        /// </summary>
        public bool IsClick { get; private set; }

        /// <summary>
        /// 指定のキーを押したとき true を取得する。
        /// </summary>
        public bool IsDown { get; private set; }

        /// <summary>
        /// 指定のキーを離したとき true を取得する。
        /// </summary>
        public bool IsUp { get; private set; }

        /// <summary>
        /// 指定のキーが押されているとき true を取得する。
        /// </summary>
        public bool IsHeldDown { get; private set; }

        public bool BalloonIsClick => _balloon.IsClick;

        public KeyCode Key { get; set; }
        public bool ReadyToClick { get; set; }
        public bool DragToShowKeys { get; set; } = false;
        public bool DragKeyIsVisible => _keysCanvasGroup.interactable;

        private bool readyToDrag;
        private AnchorKeyItem enteredItem;
        private RogueDirectionalSpriteTable keySpriteTable;

        public delegate void SelectItem(AnchorKeyItem item);

        public void Initialize(bool keyMode, RogueDirectionalSpriteTable keySpriteTable = null)
        {
            _image.color = RoguegardSettings.White;
            _image.enabled = !keyMode;
            _centerKey.Image.enabled = false;
            _keysCanvasGroup.alpha = keyMode ? 1f : 0f;
            _keysCanvasGroup.interactable = keyMode;
            _keysCanvasGroup.blocksRaycasts = keyMode;
            _balloon.SetEnabled(false, null);
            SetDrag(false);

            if (keySpriteTable != null)
            {
                _upperLeftKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.UpperLeft));
                _upKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.Up));
                _upperRightKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.UpperRight));
                _leftKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.Left));
                _rightKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.Right));
                _lowerLeftKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.LowerLeft));
                _downKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.Down));
                _lowerRightKey.SetSprite(keySpriteTable.GetSprite(RogueDirection.LowerRight));
            }
        }

        private void Start()
        {
            SelectItem setItem = (x) => enteredItem = x;
            _upperLeftKey.OnEnter += setItem;
            _upKey.OnEnter += setItem;
            _upperRightKey.OnEnter += setItem;
            _leftKey.OnEnter += setItem;
            _rightKey.OnEnter += setItem;
            _lowerLeftKey.OnEnter += setItem;
            _downKey.OnEnter += setItem;
            _lowerRightKey.OnEnter += setItem;
        }

        private void Update()
        {
            if (Input.GetKeyDown(Key))
            {
                Down();
                readyToDrag = false; // キー入力ではドラッグ無効
            }
            if (Input.GetKeyUp(Key))
            {
                Up();
            }
        }

        private void Down()
        {
            IsDown = true;
            IsHeldDown = true;
            ReadyToClick = true;
            readyToDrag = true;

            _image.color = RoguegardSettings.Gray;
        }

        private void Up()
        {
            SetDrag(false);
            IsUp = true;
            IsHeldDown = false;
            readyToDrag = false;
            enteredItem = null;
            if (ReadyToClick)
            {
                IsClick = true;
                ReadyToClick = false;
            }

            _image.color = RoguegardSettings.White;
        }

        public void CommandUpdate()
        {
            IsDown = false;
            IsUp = false;
            IsClick = false;
            _balloon.CommandUpdate();
        }

        public void SetEnabled(bool enabled)
        {
            Up();
            _canvasGroup.alpha = enabled ? 1f : 0f;
            _canvasGroup.interactable = enabled;
            _canvasGroup.blocksRaycasts = enabled;
            _balloon.SetEnabled(false, null);
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public bool TryGetDirection(out RogueDirection direction)
        {
            if (!IsHeldDown || enteredItem == null)
            {
                direction = default;
                return false;
            }
            else if (enteredItem == _rightKey)
            {
                direction = RogueDirection.Right;
                return true;
            }
            else if (enteredItem == _upperRightKey)
            {
                direction = RogueDirection.UpperRight;
                return true;
            }
            else if (enteredItem == _upKey)
            {
                direction = RogueDirection.Up;
                return true;
            }
            else if (enteredItem == _upperLeftKey)
            {
                direction = RogueDirection.UpperLeft;
                return true;
            }
            else if (enteredItem == _leftKey)
            {
                direction = RogueDirection.Left;
                return true;
            }
            else if (enteredItem == _lowerLeftKey)
            {
                direction = RogueDirection.LowerLeft;
                return true;
            }
            else if (enteredItem == _downKey)
            {
                direction = RogueDirection.Down;
                return true;
            }
            else if (enteredItem == _lowerRightKey)
            {
                direction = RogueDirection.LowerRight;
                return true;
            }
            else throw new RogueException();
        }

        private void SetDrag(bool drag)
        {
            if (!DragToShowKeys || !_image.enabled) return;

            _keysCanvasGroup.alpha = drag ? 1f : 0f;
            _keysCanvasGroup.interactable = drag;
            _keysCanvasGroup.blocksRaycasts = drag;
        }

        public void SetBalloon(bool enabled, Sprite icon)
        {
            _balloon.SetEnabled(enabled, icon);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Down();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Up();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (readyToDrag)
            {
                // ボタンの外にスワイプしたらクリックを無効化する。
                ReadyToClick = false;
                readyToDrag = false;
                SetDrag(true);
                _image.color = RoguegardSettings.White;
            }
        }
    }
}
