using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
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
                if (model is EvtFairyInfo.Point point) return point.ChartCmn;
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
                    eventFairyInfo.AddPoint();
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
                return NamingEffect.Get(fairy)?.Naming;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var fairy = arg.TargetObj;
                default(IActiveRogueMethodCaller).Affect(fairy, 1f, NamingEffect.Callback);
                NamingEffect.Get(fairy).Naming = value;
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
                return eventFairyInfo.RelatedChart;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                eventFairyInfo.RelatedChart = value;
            }
        }

        private class PointMenu : BaseScrollModelsMenu<object>
        {
            private static readonly object[] models = new object[]
            {
                new ConditionID(),
                new AdditionalConditionID(),
                new AppearanceAssetID(),
                new CategoryMenu(),
                new CmnID()
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
                return point.ChartCmn;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.ChartCmn = value;
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
                return point.IfCmn.Cmn;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.IfCmn.Cmn = value;
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
                return point.Sprite;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.Sprite = value;
            }
        }

        private class CategoryMenu : BaseScrollModelsMenu<EvtFairyInfo.Category>, IModelsMenuChoice
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            private static readonly EvtFairyInfo.Category[] models = new EvtFairyInfo.Category[]
            {
                EvtFairyInfo.Category.ApplyTool,
                EvtFairyInfo.Category.Trap
            };

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "カテゴリ";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var point = (EvtFairyInfo.Point)arg.Other;
                root.OpenMenu(this, null, null, new(other: point));
            }

            protected override Spanning<EvtFairyInfo.Category> GetModels(
                IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return models;
            }

            protected override string GetItemName(
                EvtFairyInfo.Category model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return model.ToString();
            }

            protected override void ActivateItem(
                EvtFairyInfo.Category model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var point = (EvtFairyInfo.Point)arg.Other;
                point.Category = model;
                root.Back();
            }
        }

        private class CmnID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "Cmn";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                return point.Cmn.Cmn;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var point = (EvtFairyInfo.Point)arg.Other;
                point.Cmn.Cmn = value;
            }
        }
    }
}
