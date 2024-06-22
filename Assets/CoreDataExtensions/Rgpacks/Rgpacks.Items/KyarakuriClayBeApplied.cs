using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class KyarakuriClayBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var info = KyarakuriClayInfo.Get(self);
            if (info == null)
            {
                KyarakuriClayInfo.SetTo(self);
            }

            menu ??= new();
            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self, other: info));
            return false;
        }

        private class Menu : IModelsMenu, IModelListPresenter
        {
            private static readonly List<object> models = new List<object>()
            {
                new AssetID(),
                new RaceWeight()
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var options = root.Get(DeviceKw.MenuOptions);
                options.OpenView(this, models, root, self, user, arg);
                options.SetPosition(0f);
                ExitModelsMenuChoice.OpenLeftAnchorExit(root);
            }

            public string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ChoiceListPresenter.Instance.GetItemName(model, root, self, user, arg);
            }

            public void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                ChoiceListPresenter.Instance.ActivateItem(model, root, self, user, arg);
            }

            private class AssetID : IModelsMenuOptionText
            {
                public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "アセットID";

                public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var kyarakuriClay = arg.TargetObj;
                    return NamingEffect.Get(kyarakuriClay)?.Naming;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var kyarakuriClay = arg.TargetObj;
                    default(IActiveRogueMethodCaller).Affect(kyarakuriClay, 1f, NamingEffect.Callback);
                    NamingEffect.Get(kyarakuriClay).Naming = value;
                }
            }

            private class RaceWeight : IModelsMenuChoice
            {
                private static readonly PropertiedCmnMenu nextMenu = new();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "重さ";

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var kyarakuriClayInfo = (KyarakuriClayInfo)arg.Other;
                    root.OpenMenu(nextMenu, self, null, new(other: kyarakuriClayInfo.RaceWeight));
                }
            }
        }
    }
}
