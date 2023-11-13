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
    public abstract class ModelsMenuView : MonoBehaviour, IModelsMenuView
    {
        public abstract CanvasGroup CanvasGroup { get; }

        public IModelsMenuRoot Root { get; private set; }

        public RogueObj Self { get; private set; }

        public RogueObj User { get; private set; }

        public RogueMethodArgument Arg { get; private set; }

        protected void SetArg(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            Root = root;
            Self = self;
            User = user;
            Arg = arg;
        }

        public abstract void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        public abstract float GetPosition();

        public abstract void SetPosition(float position);
    }
}
