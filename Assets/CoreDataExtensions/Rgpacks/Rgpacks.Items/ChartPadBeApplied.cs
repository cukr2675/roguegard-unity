using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
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

            private readonly ScrollViewTemplate<object, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                ScrollSubViewName = StandardSubViewTable.WidgetsName,
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var chartPad = arg.Arg.TargetObj;
                var chartPadInfo = ChartPadInfo.Get(chartPad);
                elms.Clear();
                for (int i = 0; i < chartPadInfo.Cmns.Count; i++)
                {
                    elms.Add(chartPadInfo.Cmns[i]);
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .InsertNext(
                        new object[]
                        {
                            "アセットID",
                            InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
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

                    .Append(SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(
                        "+ イベントを追加", (manager, arg) =>
                        {
                            var chartPad = arg.Arg.TargetObj;
                            var chartPadInfo = ChartPadInfo.Get(chartPad);

                            chartPadInfo.AddCmn();
                            manager.Reopen();
                        }))

                    .ElementNameFrom((element, manager, arg) =>
                    {
                        if (element == null) return "+ イベントを追加";
                        else return ((PropertiedCmnData)element).Cmn;
                    })

                    .VariableOnce(out var nextMenu, new PropertiedCmnMenu())
                    .OnClickElement((element, manager, arg) =>
                    {
                        var chartPad = arg.Arg.TargetObj;
                        var chartPadInfo = ChartPadInfo.Get(chartPad);

                        if (element == null)
                        {
                            chartPadInfo.AddCmn();
                            manager.Reopen();
                        }
                        else
                        {
                            var cmnData = (PropertiedCmnData)element;
                            manager.PushMenuScreen(nextMenu, arg.Self, other: cmnData);
                        }
                    })

                    .Build();
            }
        }
    }
}
