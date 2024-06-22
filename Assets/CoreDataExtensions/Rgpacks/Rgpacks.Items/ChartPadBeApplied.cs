using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class ChartPadBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var chartPadInfo = ChartPadInfo.Get(self);
            if (chartPadInfo == null)
            {
                ChartPadInfo.SetTo(self);
            }

            menu ??= new();
            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : IListMenu, IElementPresenter
        {
            private static readonly List<object> elms = new();
            private static readonly PropertiedCmnMenu nextMenu = new();
            private static readonly object assetId = new AssetID();

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);
                elms.Clear();
                elms.Add(assetId);
                for (int i = 0; i < chartPadInfo.Cmns.Count; i++)
                {
                    elms.Add(chartPadInfo.Cmns[i]);
                }
                elms.Add(null);
                var options = manager.GetView(DeviceKw.MenuOptions);
                options.OpenView(this, elms, manager, self, user, new(targetObj: chartPad));
                options.SetPosition(0f);
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null) return "+ イベントを追加";
                else return ((PropertiedCmnData)element).Cmn;
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);

                if (element == null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    chartPadInfo.AddCmn();
                    manager.Reopen(null, null, arg);
                }
                else
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var cmnData = (PropertiedCmnData)element;
                    manager.OpenMenu(nextMenu, self, null, new(other: cmnData));
                }
            }

            private class AssetID : IOptionsMenuText
            {
                public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

                public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "アセットID";

                public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var chartPad = arg.TargetObj;
                    return NamingEffect.Get(chartPad)?.Naming;
                }

                public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var chartPad = arg.TargetObj;
                    default(IActiveRogueMethodCaller).Affect(chartPad, 1f, NamingEffect.Callback);
                    NamingEffect.Get(chartPad).Naming = value;
                }
            }
        }
    }
}
