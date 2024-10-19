using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TalkWindow : ElementsView//, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private MessageText _text = null;
        [SerializeField] private CanvasGroup _talkSelectCanvasGroup = null;

        //public override CanvasGroup CanvasGroup => _canvasGroup;

        //public bool IsScrollingNow => _text.IsScrollingNow || _text.IsTalkingNow;

        //public bool IsTypewritingNow => _text.IsTypewritingNow;

        //public bool IsShow => _canvasGroup.blocksRaycasts;

        //private IElementPresenter presenter;
        //private object source;
        //private bool waitEndOfTalk;

        //public override float GetPosition() => 0f;
        //public override void SetPosition(float position) { }

        //public override void OpenView<T>(
        //    IElementPresenter presenter, Spanning<T> list, object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //{
        //    this.presenter = presenter;
        //    source = list[0];
        //    SetArg(manager, self, user, arg);
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public void Input()
        //{
        //    if (!_text.WaitsInput || _text.IsScrollingNow) return;

        //    _text.Input();
        //    if (!_text.IsTalkingNow)
        //    {
        //        MenuController.Show(_canvasGroup, false);
        //        _text.Clear();

        //        //presenter?.ActivateItem(source, Root, Self, User, Arg);
        //        presenter = null;
        //    }
        //}

        //public void StartTalk()
        //{
        //    _text.Clear();
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public void Append(string text)
        //{
        //    _text.Append(text);
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public void Append(int integer)
        //{
        //    _text.Append(integer);
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public void Append(float number)
        //{
        //    _text.Append(number);
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public void AppendHorizontalRule()
        //{
        //    _text.AppendHorizontalRule();
        //}

        //public void WaitEndOfTalk()
        //{
        //    MenuController.Show(_talkSelectCanvasGroup, false);
        //    waitEndOfTalk = true;
        //}

        //public void Clear()
        //{
        //    _text.Clear();
        //    MenuController.Show(_canvasGroup, false);
        //    presenter = null;
        //}

        //public void UpdateUI(int deltaTime)
        //{
        //    _text.UpdateUI(deltaTime);

        //    if (waitEndOfTalk && _text.WaitsInput)
        //    {
        //        MenuController.Show(_talkSelectCanvasGroup, true);
        //        waitEndOfTalk = false;
        //        _text.Input();
        //    }
        //}

        //void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        //{
        //    Input();
        //}
    }
}
