//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Roguegard.Device
//{
//    public class ElementsViewPosition
//    {
//        private readonly IKeyword viewKeyword;

//        private float prevPosition;
//        private object prevPositionHolder;

//        public ElementsViewPosition(IKeyword viewKeyword)
//        {
//            this.viewKeyword = viewKeyword;
//        }

//        /// <summary>
//        /// スクロール位置を記憶する
//        /// </summary>
//        public void Save(IListMenuManager manager, object positionHolder)
//        {
//            var view = manager.GetView(viewKeyword);
//            prevPosition = view.GetPosition();
//            prevPositionHolder = positionHolder;
//        }

//        /// <summary>
//        /// 前回の <paramref name="value"/> と同じであれば同じ位置までスクロールし、違うなら <paramref name="defaultPosition"/> までスクロールする。
//        /// </summary>
//        public bool Load(IListMenuManager manager, object positionHolder, float defaultPosition = 0f)
//        {
//            var usePrevPosition = positionHolder == prevPositionHolder;
//            var position = usePrevPosition ? prevPosition : defaultPosition;
//            var view = manager.GetView(viewKeyword);
//            view.SetPosition(position);
//            return usePrevPosition;
//        }

//        public void Set(IListMenuManager manager, object positionHolder, float position)
//        {
//            var view = manager.GetView(viewKeyword);
//            prevPosition = position;
//            prevPositionHolder = positionHolder;
//            view.SetPosition(position);
//        }

//        public void Reset()
//        {
//            prevPosition = 0f;
//            prevPositionHolder = null;
//        }

//        public IListMenuSelectOption GetExitSelectOptionSelf()
//        {
//            return new ExitSelectOption() { parent = this, valueType = ValueType.Self };
//        }

//        public IListMenuSelectOption GetExitSelectOptionTargetObj()
//        {
//            return new ExitSelectOption() { parent = this, valueType = ValueType.TargetObj };
//        }

//        private class ExitSelectOption : BaseListMenuSelectOption
//        {
//            public override string Name => ExitListMenuSelectOption.Instance.Name;

//            public ElementsViewPosition parent;
//            public ValueType valueType;

//            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//            {
//                switch (valueType)
//                {
//                    case ValueType.Self:
//                        parent.Save(manager, self);
//                        break;
//                    case ValueType.TargetObj:
//                        parent.Save(manager, arg.TargetObj);
//                        break;
//                }
//                ExitListMenuSelectOption.Instance.Activate(manager, self, user, arg);
//            }
//        }

//        private enum ValueType
//        {
//            Self,
//            TargetObj
//        }
//    }
//}
