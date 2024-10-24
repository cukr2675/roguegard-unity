//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using System.Text.RegularExpressions;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using TMPro;
//using ListingMF;
//using Roguegard;
//using Roguegard.Device;

//namespace RoguegardUnity
//{
//    public class TitleCredit : ElementsSubView, IPointerClickHandler
//    {
//        [SerializeField] private CanvasGroup _canvasGroup = null;
//        [SerializeField] private TMP_Text _text = null;
//        [SerializeField] private ScrollRect _scrollRect = null;
//        [SerializeField] private ButtonViewElement _exitButton = null;

//        private URLTalk urlTalk;

//        public void Initialize()
//        {
//            urlTalk = new URLTalk();
//            _exitButton.Initialize(this);
//            _exitButton.SetElement(SelectOptionHandler.Instance, ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("<", (manager, arg) =>
//            {
//                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
//                MenuController.Show(_canvasGroup, false);
//            }));
//        }

//        public override void SetParameters(
//            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
//            ref IElementsSubViewStateProvider stateProvider)
//        {
//            SetArg(manager, arg);
//        }

//        public void Show(CreditData credit, IListMenuManager manager)
//        {
//            // 文字列にリンクを貼ったものを表示
//            _text.text = Regex.Replace(credit.Details, @"(https?://\S+)", "<color=#8080ff><u><link>$1</link></u></color>");

//            MenuController.Show(_canvasGroup, true);
//        }

//        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
//        {
//            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);
//            if (linkIndex == -1) return;

//            var linkInfo = _text.textInfo.linkInfo[linkIndex];
//            var url = linkInfo.GetLinkText();
//            //Application.OpenURL(url);
//            ((RogueMenuManager)Manager).PushMenuScreen(urlTalk, other: url);
//        }

//        private class URLTalk : RogueMenuScreen
//        {
//            private readonly SpeechBoxViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
//            {
//            };

//            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
//            {
//                view.Show($"{arg.Arg.Other} へ移動しますか？", manager, arg)
//                    ?.Option(":Yes", (manager, arg) =>
//                    {
//                        manager.HandleClickBack();

//                        var url = (string)arg.Arg.Other;
//                        Application.OpenURL(url);
//                    })
//                    .Exit()
//                    .Build();
//            }
//        }
//    }
//}
