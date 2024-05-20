using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Device;

namespace Roguegard
{
    public class ChartPadBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : IModelsMenu, IModelListPresenter
        {
            private static readonly List<object> models = new();
            private static readonly RgpackReferenceMenu nextMenu = new();
            private static readonly object assetId = new AssetID();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var chart = RogueChartInfo.GetChart(chartPad);
                models.Clear();
                models.Add(assetId);
                if (chart != null)
                {
                    for (int i = 0; i < chart.Cmns.Count; i++)
                    {
                        models.Add(chart.Cmns[i]);
                    }
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
                else return ((RgpackReference)model).AssetID;
            }

            public void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var chart = RogueChartInfo.GetChart(chartPad);

                if (model == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    if (chart == null)
                    {
                        RogueChartInfo.SetChartTo(chartPad, new RogueChart("Chart1", new RgpackReference[0]));
                        chart = RogueChartInfo.GetChart(chartPad);
                    }
                    var newEvents = new RgpackReference[chart.Cmns.Count + 1];
                    for (int i = 0; i < chart.Cmns.Count; i++)
                    {
                        newEvents[i] = chart.Cmns[i];
                    }
                    newEvents[newEvents.Length - 1] = new RgpackReference(null, null);
                    RogueChartInfo.SetChartTo(chartPad, new RogueChart(chart.ID, newEvents));
                    root.Reopen(null, null, arg);
                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var index = chart.Cmns.IndexOf((RgpackReference)model);
                    Debug.Log(chartPad);
                    root.OpenMenu(nextMenu, null, null, new(targetObj: chartPad, count: index));
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
                    var chart = RogueChartInfo.GetChart(chartPad);
                    return chart.ID;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var chartPad = arg.TargetObj;
                    var chart = RogueChartInfo.GetChart(chartPad);
                    RogueChartInfo.SetChartTo(chartPad, new RogueChart(value, chart.Cmns.ToArray()));
                }
            }
        }

        private class RgpackReferenceMenu : IModelsMenu
        {
            private readonly object[] models = new object[]
            {
                new AssetID(),
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chartPad = arg.TargetObj;
                var index = arg.Count;

                var options = root.Get(DeviceKw.MenuOptions);
                options.OpenView(ChoiceListPresenter.Instance, models, root, null, null, new(targetObj: chartPad, count: index));
                options.SetPosition(0f);
                ExitModelsMenuChoice.OpenLeftAnchorExit(root);
            }

            private class AssetID : IModelsMenuOptionText
            {
                public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "アセットID";

                public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var chartPad = arg.TargetObj;
                    var chart = RogueChartInfo.GetChart(chartPad);
                    var index = arg.Count;
                    var rgpackReference = chart.Cmns[index];
                    return rgpackReference.AssetID;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var chartPad = arg.TargetObj;
                    var chart = RogueChartInfo.GetChart(chartPad);
                    var index = arg.Count;
                    var events = chart.Cmns.ToArray();
                    var rgpackReference = events[index];
                    events[index] = new RgpackReference(rgpackReference.RgpackID, value);
                    RogueChartInfo.SetChartTo(chartPad, new RogueChart(chart.ID, events));
                }
            }
        }
    }
}
