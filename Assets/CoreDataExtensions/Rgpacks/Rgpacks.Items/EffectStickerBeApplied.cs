using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class EffectStickerBeApplied : BaseApplyRogueMethod
    {
        private static EffectStickerScreen effectStickerScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            effectStickerScreen ??= new();
            var effectStickerInfo = EffectStickerInfo.Get(self);
            if (effectStickerInfo == null)
            {
                EffectStickerInfo.SetTo(self);
            }

            RogueDevice.Primary.AddScreen(effectStickerScreen, user, null, new(targetObj: self));
            return false;
        }

        private class EffectStickerScreen : RogueListuiScreen
        {
            private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.Show(System.Array.Empty<object>(), manager, arg)
                    ?
                    .TailStack("アセットID", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => NamingEffect.Get(arg.Arg.TargetObj)?.Naming,
                        (manager, arg, value) => {
                            var sticker = arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(sticker, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(sticker).Naming = value;
                        }))

                    .VarOnce(out var cmnScreen, new PropertiedCmnMenuScreen())
                    .Tail.Option("Update", (manager, arg) => manager.PushScreen(cmnScreen, arg.Self, other: EffectStickerInfo.Get(arg.Arg.TargetObj).Update))

                    .TailStack("スプライト", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => EffectStickerInfo.Get(arg.Arg.TargetObj).Sprite,
                        (manager, arg, value) =>  EffectStickerInfo.Get(arg.Arg.TargetObj).Sprite = value))

                    .Build();
            }
        }
    }
}
