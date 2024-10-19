//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Roguegard.Device
//{
//    public class ExitListMenuSelectOption : BaseListMenuSelectOption
//    {
//        public static ExitListMenuSelectOption Instance { get; } = new ExitListMenuSelectOption(null);

//        private static readonly object[] single = new[] { Instance };

//        public override string Name => ":Exit";

//        private readonly ListMenuAction action;

//        public ExitListMenuSelectOption(ListMenuAction action)
//        {
//            this.action = action;
//        }

//        public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            action?.Invoke(manager, self, user, arg);
//            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
//            manager.Back();
//        }

//        public static void OpenLeftAnchorExit(IListMenuManager manager)
//        {
//            var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
//            leftAnchor.OpenView(SelectOptionPresenter.Instance, single, manager, null, null, RogueMethodArgument.Identity);
//        }
//    }
//}
