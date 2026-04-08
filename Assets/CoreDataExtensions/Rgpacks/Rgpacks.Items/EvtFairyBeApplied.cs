using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;

namespace Roguegard.Rgpacks
{
    public class EvtFairyBeApplied : BaseApplyRogueMethod
    {
        private static EvtFairyScreen evtFairyScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            evtFairyScreen ??= new();
            var characterCreationInfo = EvtFairyInfo.Get(self);
            if (characterCreationInfo == null)
            {
                EvtFairyInfo.SetTo(self);
            }

            RogueDevice.Primary.AddScreen(evtFairyScreen, user, null, new(targetObj: self));
            return false;
        }

        private class EvtFairyScreen : RogueListuiScreen
        {
            private static readonly List<object> list = new();
            private readonly VariableWidgetsMenuViewData<MMgr> view = new()
            {
            };

            private static readonly PageScreen nextScreen = new();

            public EvtFairyScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    var fairy = Arg.Arg.TargetObj;
                    var eventFairyInfo = EvtFairyInfo.Get(fairy);
                    list.Clear();
                    foreach (var page in eventFairyInfo.Pages)
                    {
                        list.Add(SelectOption.Create<MMgr, MArg>(
                            page.ChartCmn ?? "",
                            (manager, arg) => { manager.PushScreen(nextScreen, arg.Self, other: page); }));
                    }

                    view.Show(list, manager)
                    ?
                    .HeadStack("アセットID", InputFieldWidgetOption.Create<MMgr>(
                        _ => NamingEffect.Get(Arg.Arg.TargetObj)?.Naming,
                        value =>
                        {
                            var fairy = Arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(fairy, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(fairy).Naming = value;
                        }))

                    .HeadStack("チャートID", InputFieldWidgetOption.Create<MMgr>(
                        _ => EvtFairyInfo.Get(Arg.Arg.TargetObj).RelatedChart,
                        value => EvtFairyInfo.Get(Arg.Arg.TargetObj).RelatedChart = value))

                    .Tail.Option("+ ページを追加", (manager) =>
                    {
                        var fairy = Arg.Arg.TargetObj;
                        var eventFairyInfo = EvtFairyInfo.Get(fairy);
                        eventFairyInfo.AddPage();
                        manager.Reopen();
                    })

                    .Build();
                };
            }
        }

        private class PageScreen : RogueListuiScreen
        {
            private readonly VariableWidgetsMenuViewData<MMgr> view = new()
            {
            };

            public PageScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(System.Array.Empty<object>(), manager)
                    ?
                    .TailStack("条件Cmn", InputFieldWidgetOption.Create<MMgr>(
                        _ => ((EvtFairyInfo.Page)Arg.Arg.Other).ChartCmn,
                        value => ((EvtFairyInfo.Page)Arg.Arg.Other).ChartCmn = value))

                    .TailStack("追加条件Cmn", InputFieldWidgetOption.Create<MMgr>(
                        _ => ((EvtFairyInfo.Page)Arg.Arg.Other).IfCmn.Cmn,
                        value => ((EvtFairyInfo.Page)Arg.Arg.Other).IfCmn.Cmn = value))

                    .TailStack("見た目アセットID", InputFieldWidgetOption.Create<MMgr>(
                        _ => ((EvtFairyInfo.Page)Arg.Arg.Other).Sprite,
                        value => ((EvtFairyInfo.Page)Arg.Arg.Other).Sprite = value))

                    .Tail.Option("カテゴリ", new CategoryScreen(), () => Arg)

                    .VarOnce(out var cmnScreen, new PropertiedCmnMenuScreen())
                    .Tail.Option("Cmn", m => m.PushScreen(cmnScreen, Arg.Self, other: ((EvtFairyInfo.Page)Arg.Arg.Other).Cmn))

                    .Build();
                };
            }
        }

        private class CategoryScreen : RogueListuiScreen
        {
            private static readonly object[] categories = new object[]
            {
                EvtFairyCategory.ApplyTool,
                EvtFairyCategory.Trap
            };

            private readonly ScrollMenuViewData<object, MMgr> view = new()
            {
            };

            public CategoryScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(categories, manager)
                    ?
                    .NameFrom(category => category.ToString())
                    .OnClick((category, manager) =>
                    {
                        var page = (EvtFairyInfo.Page)Arg.Arg.Other;
                        page.Category = (EvtFairyCategory)category;
                        manager.PopScreen();
                    })

                    .Build();
                };
            }
        }
    }
}
