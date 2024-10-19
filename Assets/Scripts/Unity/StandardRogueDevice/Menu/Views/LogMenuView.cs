using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class LogMenuView : ElementsView//, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private MessageText _text = null;
        [SerializeField] private ScrollRect _scrollRect = null;

        //public override CanvasGroup CanvasGroup => _canvasGroup;

        //private IElementPresenter presenter;
        //private object source;

        //public override void OpenView<T>(
        //    IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //{
        //    this.presenter = presenter;
        //    source = list[0];
        //    SetArg(manager, self, user, arg);
        //    _scrollRect.verticalNormalizedPosition = 0f; // 常に一番下から表示
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public override float GetPosition() => 0f;
        //public override void SetPosition(float position) { }

        //public void Append(string text)
        //{
        //    _text.Append(text);
        //    _text.EndScroll();
        //}

        //public void Append(int integer)
        //{
        //    _text.Append(integer);
        //    _text.EndScroll();
        //}

        //public void Append(float number)
        //{
        //    _text.Append(number);
        //    _text.EndScroll();
        //}

        //public void AppendHorizontalRule()
        //{
        //    _text.AppendHorizontalRule();
        //    _text.EndScroll();
        //}

        //public void Clear()
        //{
        //    _text.Clear();
        //}

        //void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        //{
        //    presenter.ActivateItem(source, Root, Self, User, Arg);
        //}
    }
}
