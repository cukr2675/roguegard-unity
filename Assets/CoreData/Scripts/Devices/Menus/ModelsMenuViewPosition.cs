using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ModelsMenuViewPosition
    {
        private readonly IKeyword viewKeyword;

        private float prevPosition;
        private object prevPositionHolder;

        public ModelsMenuViewPosition(IKeyword viewKeyword)
        {
            this.viewKeyword = viewKeyword;
        }

        /// <summary>
        /// スクロール位置を記憶する
        /// </summary>
        public void Save(IModelsMenuRoot root, object positionHolder)
        {
            var view = root.Get(viewKeyword);
            prevPosition = view.GetPosition();
            prevPositionHolder = positionHolder;
        }

        /// <summary>
        /// 前回の <paramref name="value"/> と同じであれば同じ位置までスクロールし、違うなら <paramref name="defaultPosition"/> までスクロールする。
        /// </summary>
        public bool Load(IModelsMenuRoot root, object positionHolder, float defaultPosition = 0f)
        {
            var usePrevPosition = positionHolder == prevPositionHolder;
            var position = usePrevPosition ? prevPosition : defaultPosition;
            var view = root.Get(viewKeyword);
            view.SetPosition(position);
            return usePrevPosition;
        }

        public void Set(IModelsMenuRoot root, object positionHolder, float position)
        {
            var view = root.Get(viewKeyword);
            prevPosition = position;
            prevPositionHolder = positionHolder;
            view.SetPosition(position);
        }

        public IModelsMenuChoice GetExitChoiceSelf()
        {
            return new ExitChoice() { parent = this, valueType = ValueType.Self };
        }

        public IModelsMenuChoice GetExitChoiceTargetObj()
        {
            return new ExitChoice() { parent = this, valueType = ValueType.TargetObj };
        }

        private class ExitChoice : IModelsMenuChoice
        {
            public ModelsMenuViewPosition parent;
            public ValueType valueType;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ExitModelsMenuChoice.Instance.GetName(root, self, user, arg);
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                switch (valueType)
                {
                    case ValueType.Self:
                        parent.Save(root, self);
                        break;
                    case ValueType.TargetObj:
                        parent.Save(root, arg.TargetObj);
                        break;
                }
                ExitModelsMenuChoice.Instance.Activate(root, self, user, arg);
            }
        }

        private enum ValueType
        {
            Self,
            TargetObj
        }
    }
}
