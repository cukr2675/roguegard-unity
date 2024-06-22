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
    public class TitleCredit : ElementsView, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ViewElementButton _exitButton = null;

        private ExitSelectOption exitSelectOption;
        private URLTalk urlTalk;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            exitSelectOption = new ExitSelectOption() { parent = this };
            urlTalk = new URLTalk();
            _exitButton.Initialize(this);
            _exitButton.SetItem(SelectOptionPresenter.Instance, exitSelectOption);
        }

        public override void OpenView<T>(
            IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(manager, self, user, arg);
        }

        public override float GetPosition()
        {
            return 0f;
        }

        public override void SetPosition(float position)
        {
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        public void Show(CreditData credit, IListMenuManager manager)
        {
            SetArg(manager, null, null, RogueMethodArgument.Identity);

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
            Root.OpenMenuAsDialog(urlTalk, Self, User, new(other: url));
        }

        private class ExitSelectOption : BaseListMenuSelectOption
        {
            public override string Name => ExitListMenuSelectOption.Instance.Name;

            public TitleCredit parent;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                MenuController.Show(parent._canvasGroup, false);
            }
        }

        private class URLTalk : IListMenu
        {
            private static readonly object[] elms = new object[]
            {
                new JumpSelectOption(),
                ExitListMenuSelectOption.Instance
            };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.GetView(DeviceKw.MenuTalkSelect).OpenView(SelectOptionPresenter.Instance, elms, manager, self, user, arg);
            }

            private class JumpSelectOption : BaseListMenuSelectOption
            {
                public override string Name => ":Yes";

                public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.Back();

                    var url = (string)arg.Other;
                    Application.OpenURL(url);
                }
            }
        }
    }
}
