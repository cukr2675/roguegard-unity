using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TitleCredit : ModelsMenuView, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        private ExitChoice exitChoice;
        private URLTalk urlTalk;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            exitChoice = new ExitChoice() { parent = this };
            urlTalk = new URLTalk();
            _exitButton.Initialize(this);
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, exitChoice);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
        }

        public override float GetPosition()
        {
            return 0f;
        }

        public override void SetPosition(float position)
        {
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        public void Show(CreditData credit, IModelsMenuRoot root)
        {
            SetArg(root, null, null, RogueMethodArgument.Identity);

            // 文字列にリンクを貼ったものを表示
            _text.text = Regex.Replace(credit.Details, @"(https?://\S+)", "<color=#8080ff><u><link>$1</link></u></color>");

            MenuController.Show(_canvasGroup, true);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);
            if (linkIndex == -1) return;

            var linkInfo = _text.textInfo.linkInfo[linkIndex];
            var url = linkInfo.GetLinkText();
            //Application.OpenURL(url);
            Root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            Root.AddInt(DeviceKw.StartTalk, 0);
            Root.AddObject(DeviceKw.AppendText, $"{url} へ移動しますか？");
            Root.AddInt(DeviceKw.WaitEndOfTalk, 0);
            Root.OpenMenuAsDialog(urlTalk, Self, User, new(other: url), Arg);
        }

        private class ExitChoice : IModelsMenuChoice
        {
            public TitleCredit parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "<";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                MenuController.Show(parent._canvasGroup, false);
            }
        }

        private class URLTalk : IModelsMenu
        {
            private static readonly object[] models = new object[]
            {
                new JumpChoice(),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
            }

            private class JumpChoice : IModelsMenuChoice
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "はい";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.Back();

                    var url = (string)arg.Other;
                    Application.OpenURL(url);
                }
            }
        }
    }
}
