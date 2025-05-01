using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
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
            private static readonly PageMenu nextMenu = new();

            private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var fairy = arg.Arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                elms.Clear();
                for (int i = 0; i < eventFairyInfo.Pages.Count; i++)
                {
                    var page = eventFairyInfo.Pages[i];
                    elms.Add(
                        SelectOption.Create<MMgr, MArg>(
                            page.ChartCmn ?? "",
                            (manager, arg) => { manager.PushMenuScreen(nextMenu, arg.Self, other: page); }));
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .Head(
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
                    .Head(
                        new object[]
                        {
                            "チャートID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => EvtFairyInfo.Get(arg.Arg.TargetObj).RelatedChart,
                                (manager, arg, value) => EvtFairyInfo.Get(arg.Arg.TargetObj).RelatedChart = value)
                        })

                    .Tail(
                        SelectOption.Create<MMgr, MArg>("+ ページを追加", (manager, arg) =>
                        {
                            var fairy = arg.Arg.TargetObj;
                            var eventFairyInfo = EvtFairyInfo.Get(fairy);
                            eventFairyInfo.AddPage();
                            manager.Reopen();
                        }))

                    .Build();
            }
        }

        private class PageMenu : RogueMenuScreen
        {
            private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(System.Array.Empty<object>(), manager, arg)
                    ?
                    .Tail(
                        new object[]
                        {
                            "条件Cmn",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Page)arg.Arg.Other).ChartCmn,
                                (manager, arg, value) => ((EvtFairyInfo.Page)arg.Arg.Other).ChartCmn = value)
                        })
                    .Tail(
                        new object[]
                        {
                            "追加条件Cmn",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Page)arg.Arg.Other).IfCmn.Cmn,
                                (manager, arg, value) => ((EvtFairyInfo.Page)arg.Arg.Other).IfCmn.Cmn = value)
                        })
                    .Tail(
                        new object[]
                        {
                            "見た目アセットID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => ((EvtFairyInfo.Page)arg.Arg.Other).Sprite,
                                (manager, arg, value) => ((EvtFairyInfo.Page)arg.Arg.Other).Sprite = value)
                        })
                    .Tail(SelectOption.Create<MMgr, MArg>("カテゴリ", new CategoryMenu()))
                    .VarOnce(out var cmnMenu, new PropertiedCmnMenu())
                    .Tail(SelectOption.Create<MMgr, MArg>(
                        "Cmn",
                        (manager, arg) => manager.PushMenuScreen(cmnMenu, arg.Self, other: ((EvtFairyInfo.Page)arg.Arg.Other).Cmn)))

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
                    .NameFrom((category, manager, arg) => category.ToString())
                    .OnClick((category, manager, arg) =>
                    {
                        var page = (EvtFairyInfo.Page)arg.Arg.Other;
                        page.Category = (EvtFairyCategory)category;
                        manager.PopMenuScreen();
                    })

                    .Build();
            }
        }
    }
}
