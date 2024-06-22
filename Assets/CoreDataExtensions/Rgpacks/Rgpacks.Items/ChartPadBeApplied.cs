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

        private class Menu : IModelsMenu, IModelListPresenter
        {
            private static readonly List<object> models = new();
            private static readonly PropertiedCmnMenu nextMenu = new();
            private static readonly object assetId = new AssetID();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);
                models.Clear();
                models.Add(assetId);
                for (int i = 0; i < chartPadInfo.Cmns.Count; i++)
                {
                    models.Add(chartPadInfo.Cmns[i]);
                }
                models.Add(null);
                var options = root.Get(DeviceKw.MenuOptions);
                options.OpenView(this, models, root, self, user, new(targetObj: chartPad));
                options.SetPosition(0f);
                ExitModelsMenuChoice.OpenLeftAnchorExit(root);
            }

            public string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "+ イベントを追加";
                else return ((PropertiedCmnData)model).Cmn;
            }

            public void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);

                if (model == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    chartPadInfo.AddCmn();
                    root.Reopen(null, null, arg);
                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var cmnData = (PropertiedCmnData)model;
                    root.OpenMenu(nextMenu, self, null, new(other: cmnData));
                }
            }

            private class AssetID : IModelsMenuOptionText
            {
                public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "アセットID";

                public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var chartPad = arg.TargetObj;
                    return NamingEffect.Get(chartPad)?.Naming;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var chartPad = arg.TargetObj;
                    default(IActiveRogueMethodCaller).Affect(chartPad, 1f, NamingEffect.Callback);
                    NamingEffect.Get(chartPad).Naming = value;
                }
            }
        }
    }
}
