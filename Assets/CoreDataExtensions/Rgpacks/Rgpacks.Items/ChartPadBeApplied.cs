using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;

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
            private static readonly List<object> elms = new();
            private static readonly PropertiedCmnMenu nextMenu = new();

            private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var chartPad = arg.Arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);
                elms.Clear();
                for (int i = 0; i < chartPadInfo.Cmns.Count; i++)
                {
                    var cmn = chartPadInfo.Cmns[i];
                    elms.Add(
                        SelectOption.Create<MMgr, MArg>(
                            cmn.Cmn ?? "[新しいコモンイベント]",
                            (manager, arg) =>
                            {
                                manager.PushMenuScreen(nextMenu, arg.Self, other: cmn);
                            }));
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .InsertNext(
                        new object[]
                        {
                            "アセットID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
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
                                })
                        })

                    .Append(SelectOption.Create<MMgr, MArg>(
                        "+ イベントを追加", (manager, arg) =>
                        {
                            var chartPad = arg.Arg.TargetObj;
                            var chartPadInfo = ChartPadInfo.Get(chartPad);

                            chartPadInfo.AddCmn();
                            manager.Reopen();
                        }))

                    .Build();
            }
        }
    }
}
