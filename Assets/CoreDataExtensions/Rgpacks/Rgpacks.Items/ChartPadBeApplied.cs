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
            private readonly VariableWidgetsMenuViewData<MMgr> view = new()
            {
            };

            private static readonly PropertiedCmnMenuScreen nextScreen = new();

            public ChartPadScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    var chartPad = Arg.Arg.TargetObj;
                    var chartPadInfo = ChartPadInfo.Get(chartPad);
                    list.Clear();
                    foreach (var cmn in chartPadInfo.Cmns)
                    {
                        list.Add(SelectOption.Create<MMgr>(
                            cmn.Cmn ?? "[新しいコモンイベント]",
                            m => m.PushScreen(nextScreen, Arg.Self, other: cmn)));
                    }

                    view.Show(list, manager)
                    ?
                    .HeadStack("アセットID", InputFieldWidgetOption.Create<MMgr>(
                        _ =>
                        {
                            var chartPad = Arg.Arg.TargetObj;
                            return NamingEffect.Get(chartPad)?.Naming;
                        },
                        value =>
                        {
                            var chartPad = Arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(chartPad, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(chartPad).Naming = value;
                        }))

                    .Tail.Option("+ イベントを追加", (manager) =>
                    {
                        var chartPad = Arg.Arg.TargetObj;
                        var chartPadInfo = ChartPadInfo.Get(chartPad);

                        chartPadInfo.AddCmn();
                        manager.Reopen();
                    })

                    .Build();
                };
            }
        }
    }
}
