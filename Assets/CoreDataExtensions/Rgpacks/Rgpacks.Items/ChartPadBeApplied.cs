using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;

namespace Roguegard.Rgpacks
{
    public class ChartPadBeApplied : BaseApplyRogueMethod
    {
        private static ChartPadScreen chartPadScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var chartPadInfo = ChartPadInfo.Get(self);
            if (chartPadInfo == null)
            {
                ChartPadInfo.SetTo(self);
            }

            chartPadScreen ??= new();
            RogueDevice.Primary.AddScreen(chartPadScreen, user, null, new(targetObj: self));
            return false;
        }

        private class ChartPadScreen : RogueListuiScreen
        {
            private static readonly List<object> list = new();
            private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
            {
            };

            private static readonly PropertiedCmnMenuScreen nextScreen = new();

            public override void OpenScreen(MMgr manager, MArg arg)
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
                            manager.PushScreen(nextScreen, arg.Self, other: cmn);
                        }));
                }

                view.Show(list, manager, arg)
                    ?
                    .HeadStack("アセットID", InputFieldWidgetOption.Create<MMgr, MArg>(
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

                    .Tail.Option("+ イベントを追加", (manager, arg) =>
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
