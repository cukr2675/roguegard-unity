using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;
using Roguegard.Device;

namespace Roguegard
{
    public class SpQuestMonolithBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            RogueDevice.Primary.AddMenu(menu, user, null, RogueMethodArgument.Identity);
            return false;
        }

        private class Menu : BaseScrollModelsMenu<object>
        {
            private static readonly object[] models = new object[]
            {
                new LeaveChoice()
            };

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return models;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ChoiceListPresenter.Instance.GetItemName(model, root, self, user, arg);
            }

            protected override void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                ChoiceListPresenter.Instance.ActivateItem(model, root, self, user, arg);
            }
        }

        private class LeaveChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "アトリエから出る";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                default(IActiveRogueMethodCaller).LocateSavePoint(self, null, 0f, RogueWorldSavePointInfo.Instance, true);
                var memberInfo = LobbyMemberList.GetMemberInfo(self);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
                root.Done();
            }
        }
    }
}
