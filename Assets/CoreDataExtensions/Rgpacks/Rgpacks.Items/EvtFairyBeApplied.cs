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
        //private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            //menu ??= new();
            var characterCreationInfo = EvtFairyInfo.Get(self);
            if (characterCreationInfo == null)
            {
                EvtFairyInfo.SetTo(self);
            }

            //RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        //private class Menu : BaseScrollListMenu<object>
        //{
        //    private static readonly List<object> elms = new();
        //    private static readonly AssetID assetID = new();
        //    private static readonly RelatedChartID relatedChartID = new();
        //    private static readonly PointMenu nextMenu = new();

        //    protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

        //    protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var fairy = arg.TargetObj;
        //        var eventFairyInfo = EvtFairyInfo.Get(fairy);
        //        elms.Clear();
        //        elms.Add(assetID);
        //        elms.Add(relatedChartID);
        //        for (int i = 0; i < eventFairyInfo.Points.Count; i++)
        //        {
        //            elms.Add(eventFairyInfo.Points[i]);
        //        }
        //        elms.Add(null);
        //        return elms;
        //    }

        //    protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (element is EvtFairyInfo.Point point) return point.ChartCmn;
        //        else return "+ Point を追加";
        //    }

        //    protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (element is EvtFairyInfo.Point point)
        //        {
        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //            manager.OpenMenu(nextMenu, null, null, new(other: point));

        //        }
        //        else
        //        {
        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //            var fairy = arg.TargetObj;
        //            var eventFairyInfo = EvtFairyInfo.Get(fairy);
        //            eventFairyInfo.AddPoint();
        //            manager.Reopen();
        //        }
        //    }
        //}

        //private class AssetID : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "アセットID";

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var fairy = arg.TargetObj;
        //        return NamingEffect.Get(fairy)?.Naming;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var fairy = arg.TargetObj;
        //        default(IActiveRogueMethodCaller).Affect(fairy, 1f, NamingEffect.Callback);
        //        NamingEffect.Get(fairy).Naming = value;
        //    }
        //}

        //private class RelatedChartID : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "チャートID";

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var fairy = arg.TargetObj;
        //        var eventFairyInfo = EvtFairyInfo.Get(fairy);
        //        return eventFairyInfo.RelatedChart;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var fairy = arg.TargetObj;
        //        var eventFairyInfo = EvtFairyInfo.Get(fairy);
        //        eventFairyInfo.RelatedChart = value;
        //    }
        //}

        //private class PointMenu : BaseScrollListMenu<object>
        //{
        //    private static readonly object[] elms = new object[]
        //    {
        //        new ConditionID(),
        //        new AdditionalConditionID(),
        //        new AppearanceAssetID(),
        //        new CategoryMenu(),
        //        new CmnID()
        //    };

        //    protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

        //    protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return elms;
        //    }

        //    protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return SelectOptionPresenter.Instance.GetItemName(element, manager, self, user, arg);
        //    }

        //    protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        SelectOptionPresenter.Instance.ActivateItem(element, manager, self, user, arg);
        //    }
        //}

        //private class ConditionID : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "条件Cmn";

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        return point.ChartCmn;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        point.ChartCmn = value;
        //    }
        //}

        //private class AdditionalConditionID : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "追加条件ID";

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        return point.IfCmn.Cmn;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        point.IfCmn.Cmn = value;
        //    }
        //}

        //private class AppearanceAssetID : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "見た目アセットID";

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        return point.Sprite;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        point.Sprite = value;
        //    }
        //}

        //private class CategoryMenu : BaseScrollListMenu<EvtFairyInfo.Category>, IListMenuSelectOption
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    private static readonly EvtFairyInfo.Category[] elms = new EvtFairyInfo.Category[]
        //    {
        //        EvtFairyInfo.Category.ApplyTool,
        //        EvtFairyInfo.Category.Trap
        //    };

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "カテゴリ";

        //    public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        manager.OpenMenu(this, null, null, new(other: point));
        //    }

        //    protected override Spanning<EvtFairyInfo.Category> GetList(
        //        IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return elms;
        //    }

        //    protected override string GetItemName(
        //        EvtFairyInfo.Category element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return element.ToString();
        //    }

        //    protected override void ActivateItem(
        //        EvtFairyInfo.Category element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        point.Category = element;
        //        manager.Back();
        //    }
        //}

        //private class CmnID : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        => "Cmn";

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        return point.Cmn.Cmn;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var point = (EvtFairyInfo.Point)arg.Other;
        //        point.Cmn.Cmn = value;
        //    }
        //}
    }
}
