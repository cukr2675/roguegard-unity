using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;

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
            private static readonly List<object> list = new();
            private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
            {
            };

            private static readonly PageMenu nextMenu = new();

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var fairy = arg.Arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                list.Clear();
                foreach (var page in eventFairyInfo.Pages)
                {
                    list.Add(SelectOption.Create<MMgr, MArg>(
                        page.ChartCmn ?? "",
                        (manager, arg) => { manager.PushMenuScreen(nextMenu, arg.Self, other: page); }));
                }

                view.Show(list, manager, arg)
                    ?
                    .HeadStack("アセットID", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => NamingEffect.Get(arg.Arg.TargetObj)?.Naming,
                        (manager, arg, value) => {
                            var fairy = arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(fairy, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(fairy).Naming = value;
                        }))

                    .HeadStack("チャートID", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => EvtFairyInfo.Get(arg.Arg.TargetObj).RelatedChart,
                        (manager, arg, value) => EvtFairyInfo.Get(arg.Arg.TargetObj).RelatedChart = value))

                    .TailOption("+ ページを追加", (manager, arg) =>
                    {
                        var fairy = arg.Arg.TargetObj;
                        var eventFairyInfo = EvtFairyInfo.Get(fairy);
                        eventFairyInfo.AddPage();
                        manager.Reopen();
                    })

                    .Build();
            }
        }

        private class PageMenu : RogueMenuScreen
        {
            private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(System.Array.Empty<object>(), manager, arg)
                    ?
                    .TailStack("条件Cmn", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => ((EvtFairyInfo.Page)arg.Arg.Other).ChartCmn,
                        (manager, arg, value) => ((EvtFairyInfo.Page)arg.Arg.Other).ChartCmn = value))

                    .TailStack("追加条件Cmn", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => ((EvtFairyInfo.Page)arg.Arg.Other).IfCmn.Cmn,
                        (manager, arg, value) => ((EvtFairyInfo.Page)arg.Arg.Other).IfCmn.Cmn = value))

                    .TailStack("見た目アセットID", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => ((EvtFairyInfo.Page)arg.Arg.Other).Sprite,
                        (manager, arg, value) => ((EvtFairyInfo.Page)arg.Arg.Other).Sprite = value))

                    .TailOption("カテゴリ", new CategoryMenu())

                    .VarOnce(out var cmnMenu, new PropertiedCmnMenu())
                    .TailOption("Cmn", (manager, arg) => manager.PushMenuScreen(cmnMenu, arg.Self, other: ((EvtFairyInfo.Page)arg.Arg.Other).Cmn))

                    .Build();
            }
        }

        private class CategoryMenu : RogueMenuScreen
        {
            private static readonly object[] categories = new object[]
            {
                EvtFairyCategory.ApplyTool,
                EvtFairyCategory.Trap
            };

            private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(categories, manager, arg)
                    ?
                    .NameFrom(category => category.ToString())
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
