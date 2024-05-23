using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Device;

namespace Roguegard
{
    public class EvtFairyBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            var characterCreationInfo = EvtFairyInfo.Get(self);
            if (characterCreationInfo == null)
            {
                EvtFairyInfo.SetTo(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : BaseScrollModelsMenu<object>
        {
            private static readonly List<object> models = new();
            private static readonly AssetID assetID = new();
            private static readonly RelatedChartID relatedChartID = new();
            private static readonly PointMenu nextMenu = new();

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                models.Clear();
                models.Add(assetID);
                models.Add(relatedChartID);
                for (int i = 0; i < eventFairyInfo.Points.Count; i++)
                {
                    models.Add(eventFairyInfo.Points[i]);
                }
                models.Add(null);
                return models;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is EvtFairyInfo.Point point) return point.ChartCmn.AssetID;
                else return "+ Point を追加";
            }

            protected override void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is EvtFairyInfo.Point point)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    root.OpenMenu(nextMenu, null, null, new(other: point));

                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var fairy = arg.TargetObj;
                    var eventFairyInfo = EvtFairyInfo.Get(fairy);
                    eventFairyInfo.AddPoint(new RgpackReference(null, null));
                    root.Reopen(null, null, arg);
                }
            }
        }

        private class AssetID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "アセットID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                return eventFairyInfo.ID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                eventFairyInfo.ID = value;
            }
        }

        private class RelatedChartID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "チャートID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                return eventFairyInfo.RelatedChart.AssetID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                eventFairyInfo.RelatedChart = new RgpackReference(eventFairyInfo.RelatedChart.RgpackID, value);
            }
        }

        private class PointMenu : BaseScrollModelsMenu<object>
        {
            private static readonly object[] models = new object[]
            {
                new ConditionID(),
                new AdditionalConditionID(),
                new AppearanceAssetID(),
            };

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

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

        private class ConditionID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "条件Cmn";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                return point.ChartCmn.AssetID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.ChartCmn = new RgpackReference(point.ChartCmn.RgpackID, value);
            }
        }

        private class AdditionalConditionID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "追加条件ID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                return point.IfCmn.AssetID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.IfCmn = new RgpackReference(point.IfCmn.RgpackID, value);
            }
        }

        private class AppearanceAssetID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "見た目アセットID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                return point.Appearance.AssetID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.Appearance = new RgpackReference(point.Appearance.RgpackID, value);
            }
        }
    }
}
