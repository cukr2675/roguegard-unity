using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TalkWindow : ModelsMenuView, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private MessageText _text = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public bool IsScrollingNow => _text.IsScrollingNow || _text.IsTalkingNow;

        public bool IsTypewritingNow => _text.IsTypewritingNow;

        public bool IsShow => _canvasGroup.blocksRaycasts;

        private IModelsMenuItemController controller;
        private object source;
        private bool waitTalk;

        public override float GetPosition() => 0f;
        public override void SetPosition(float position) { }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            controller = itemController;
            source = models[0];
            SetArg(root, self, user, arg);
            MenuController.Show(_canvasGroup, true);
        }

        public void Input()
        {
            if (!_text.WaitsInput) return;

            _text.Input();
            if (!_text.IsTalkingNow)
            {
                MenuController.Show(_canvasGroup, false);
                _text.Clear();

                controller?.Activate(source, Root, Self, User, Arg);
                controller = null;
            }
        }

        public void StartTalk()
        {
            _text.Clear();
            MenuController.Show(_canvasGroup, true);
        }

        public void Append(string text)
        {
            _text.Append(text);
            MenuController.Show(_canvasGroup, true);
        }

        public void Append(int integer)
        {
            _text.Append(integer);
            MenuController.Show(_canvasGroup, true);
        }

        public void Append(float number)
        {
            _text.Append(number);
            MenuController.Show(_canvasGroup, true);
        }

        public void AppendHorizontalRule()
        {
            _text.AppendHorizontalRule();
        }

        public void WaitTalk()
        {
            waitTalk = true;
        }

        public void Clear()
        {
            _text.Clear();
            MenuController.Show(_canvasGroup, false);
            controller = null;
        }

        public void UpdateUI(int deltaTime)
        {
            _text.UpdateUI(deltaTime);

            if (waitTalk && _text.WaitsInput)
            {
                _text.Input();
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Input();
        }
    }
}
