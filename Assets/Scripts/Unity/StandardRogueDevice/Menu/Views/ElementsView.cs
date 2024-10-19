using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    [System.Obsolete]
    public abstract class ElementsView : MonoBehaviour//, IElementsView
    {
        //public abstract CanvasGroup CanvasGroup { get; }

        public object Root { get; private set; }

        public RogueObj Self { get; private set; }

        public RogueObj User { get; private set; }

        public RogueMethodArgument Arg { get; private set; }

        protected void SetArg(object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            Root = manager;
            Self = self;
            User = user;
            Arg = arg;
        }

        //public abstract void OpenView<T>(
        //    IElementPresenter presenter, Spanning<T> list, object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        //public abstract float GetPosition();

        //public abstract void SetPosition(float position);
    }
}
