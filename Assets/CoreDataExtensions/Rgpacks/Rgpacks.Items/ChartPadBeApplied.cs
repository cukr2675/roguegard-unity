using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;

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

        private class Menu : RogueMenuScreen
        {
            private static readonly List<object> list = new();
            private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
            {
            };

            private static readonly PropertiedCmnMenu nextMenu = new();

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var chartPad = arg.Arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);
                list.Clear();
                foreach (var cmn in chartPadInfo.Cmns)
                {
                    list.Add(SelectOption.Create<MMgr, MArg>(
                        cmn.Cmn ?? "[新しいコモンイベント]",
                        (manager, arg) =>
                        {
                            manager.PushMenuScreen(nextMenu, arg.Self, other: cmn);
                        }));
                }

                view.Show(list, manager, arg)
                    ?
                    .HeadStack("アセットID", InputFieldViewWidget.CreateOption<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var chartPad = arg.Arg.TargetObj;
                            return NamingEffect.Get(chartPad)?.Naming;
                        },
                        (manager, arg, value) =>
                        {
                            var chartPad = arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(chartPad, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(chartPad).Naming = value;
                        }))

                    .TailOption("+ イベントを追加", (manager, arg) =>
                    {
                        var chartPad = arg.Arg.TargetObj;
                        var chartPadInfo = ChartPadInfo.Get(chartPad);

                        chartPadInfo.AddCmn();
                        manager.Reopen();
                    })

                    .Build();
            }
        }
    }
}
