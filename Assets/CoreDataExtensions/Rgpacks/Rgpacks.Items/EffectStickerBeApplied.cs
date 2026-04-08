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
            private readonly VariableWidgetsMenuViewData<MMgr> view = new()
            {
            };

            public EffectStickerScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(System.Array.Empty<object>(), manager)
                    ?
                    .TailStack("アセットID", InputFieldWidgetOption.Create<MMgr>(
                        _ => NamingEffect.Get(Arg.Arg.TargetObj)?.Naming,
                        value => {
                            var sticker = Arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(sticker, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(sticker).Naming = value;
                        }))

                    .VarOnce(out var cmnScreen, new PropertiedCmnMenuScreen())
                    .Tail.Option("Update", (manager) => manager.PushScreen(cmnScreen, Arg.Self, other: EffectStickerInfo.Get(Arg.Arg.TargetObj).Update))

                    .TailStack("スプライト", InputFieldWidgetOption.Create<MMgr>(
                        _ => EffectStickerInfo.Get(Arg.Arg.TargetObj).Sprite,
                        value =>  EffectStickerInfo.Get(Arg.Arg.TargetObj).Sprite = value))

                    .Build();
                };
            }
        }
    }
}
