using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
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

        private class Menu : RogueMenuScreen
        {
            private static readonly List<object> elms = new();
            private static readonly PointMenu nextMenu = new();

            private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var fairy = arg.Arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                elms.Clear();
                for (int i = 0; i < eventFairyInfo.Points.Count; i++)
                {
                    var point = eventFairyInfo.Points[i];
                    elms.Add(
                        SelectOption.Create<MMgr, MArg>(
                            point.ChartCmn ?? "",
                            (manager, arg) => { manager.PushMenuScreen(nextMenu, other: point); }));
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .InsertNext(
                        new object[]
                        {
                            "アセットID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => NamingEffect.Get(arg.Arg.TargetObj)?.Naming,
                                (manager, arg, value) => {
                                    var fairy = arg.Arg.TargetObj;
                                    default(IActiveRogueMethodCaller).Affect(fairy, 1f, NamingEffect.Callback);
                                    return NamingEffect.Get(fairy).Naming = value;
                                })
                        })
                    .InsertNext(
                        new object[]
                        {
                            "チャートID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => EvtFairyInfo.Get(arg.Arg.TargetObj).RelatedChart,
                                (manager, arg, value) => EvtFairyInfo.Get(arg.Arg.TargetObj).RelatedChart = value)
                        })

                    .Append(
                        SelectOption.Create<MMgr, MArg>("+ Point を追加", (manager, arg) =>
                        {
                            var fairy = arg.Arg.TargetObj;
                            var eventFairyInfo = EvtFairyInfo.Get(fairy);
                            eventFairyInfo.AddPoint();
                            manager.Reopen();
                        }))

                    .Build();
            }
        }

        private class PointMenu : RogueMenuScreen
        {
            private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(System.Array.Empty<object>(), manager, arg)
                    ?
                    .Append(
                        new object[]
                        {
                            "条件Cmn",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Point)arg.Arg.Other).ChartCmn,
                                (manager, arg, value) => ((EvtFairyInfo.Point)arg.Arg.Other).ChartCmn = value)
                        })
                    .Append(
                        new object[]
                        {
                            "追加条件Cmn",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Point)arg.Arg.Other).IfCmn.Cmn,
                                (manager, arg, value) => ((EvtFairyInfo.Point)arg.Arg.Other).IfCmn.Cmn = value)
                        })
                    .Append(
                        new object[]
                        {
                            "見た目アセットID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Point)arg.Arg.Other).Sprite,
                                (manager, arg, value) => ((EvtFairyInfo.Point)arg.Arg.Other).Sprite = value)
                        })
                    .Append(SelectOption.Create<MMgr, MArg>("カテゴリ", new CategoryMenu()))
                    .Append(
                        new object[]
                        {
                            "Cmn",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Point)arg.Arg.Other).Cmn.Cmn,
                                (manager, arg, value) => ((EvtFairyInfo.Point)arg.Arg.Other).Cmn.Cmn = value)
                        })

                    .Build();
            }
        }

        private class CategoryMenu : RogueMenuScreen
        {
            private static readonly object[] elms = new object[]
            {
                EvtFairyCategory.ApplyTool,
                EvtFairyCategory.Trap
            };

            private readonly ScrollViewTemplate<object, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(elms, manager, arg)
                    ?
                    .ElementNameFrom((category, manager, arg) => category.ToString())
                    .OnClickElement((category, manager, arg) =>
                    {
                        var point = (EvtFairyInfo.Point)arg.Arg.Other;
                        point.Category = (EvtFairyCategory)category;
                        manager.Back();
                    })

                    .Build();
            }
        }
    }
}
