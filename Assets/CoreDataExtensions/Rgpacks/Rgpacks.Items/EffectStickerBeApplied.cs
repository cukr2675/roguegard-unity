using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class EffectStickerBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            var effectStickerInfo = EffectStickerInfo.Get(self);
            if (effectStickerInfo == null)
            {
                EffectStickerInfo.SetTo(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : RogueMenuScreen
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
                            "アセットID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => NamingEffect.Get(arg.Arg.TargetObj)?.Naming,
                                (manager, arg, value) => {
                                    var sticker = arg.Arg.TargetObj;
                                    default(IActiveRogueMethodCaller).Affect(sticker, 1f, NamingEffect.Callback);
                                    return NamingEffect.Get(sticker).Naming = value;
                                })
                        })

                    .VarOnce(out var cmnMenu, new PropertiedCmnMenu())
                    .Append(SelectOption.Create<MMgr, MArg>(
                        "Update",
                        (manager, arg) => manager.PushMenuScreen(cmnMenu, arg.Self, other: EffectStickerInfo.Get(arg.Arg.TargetObj).Update)))

                    .Append(
                        new object[]
                        {
                            "スプライト",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => EffectStickerInfo.Get(arg.Arg.TargetObj).Sprite,
                                (manager, arg, value) =>  EffectStickerInfo.Get(arg.Arg.TargetObj).Sprite = value)
                        })

                    .Build();
            }
        }
    }
}
