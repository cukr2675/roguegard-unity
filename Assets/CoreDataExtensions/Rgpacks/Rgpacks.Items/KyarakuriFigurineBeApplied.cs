using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class KyarakuriFigurineBeApplied : BaseApplyRogueMethod
    {
        private static KyarakuriFigurineScreen kyarakuriFigurineScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            kyarakuriFigurineScreen ??= new();
            var characterCreationInfo = KyarakuriFigurineInfo.Get(self);
            if (characterCreationInfo == null)
            {
                KyarakuriFigurineInfo.SetTo(self, RoguegardSettings.CharacterCreationDatabase.LoadPreset(0));
            }

            RogueDevice.Primary.AddScreen(kyarakuriFigurineScreen, user, null, new(targetObj: self));
            return false;
        }

        private class KyarakuriFigurineScreen : RogueListuiScreen
        {
            private readonly VariableWidgetsMenuViewData<MMgr> view = new()
            {
            };

            public KyarakuriFigurineScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(System.Array.Empty<object>(), manager)
                    ?
                    .TailStack("アセットID", InputFieldWidgetOption.Create<MMgr>(
                        _ => NamingEffect.Get(Arg.Arg.TargetObj)?.Naming,
                        value =>
                        {
                            var figurine = Arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(figurine, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(figurine).Naming = value;
                        }))

                    .VarOnce(out var nextScreen, new EditScreen())
                    .Tail.Option("キャラクリ設定", (manager) =>
                    {
                        var figurine = Arg.Arg.TargetObj;
                        var characterCreationData = new CharacterCreationData(KyarakuriFigurineInfo.Get(figurine));
                        manager.PushScreen(nextScreen, Arg.Self, targetObj: figurine, other: characterCreationData);
                    })

                    .Build();
                };
            }
        }

        private class EditScreen : RogueListuiScreen
        {
            private readonly CharacterCreationViewData view = new()
            {
            };

            public EditScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(manager)
                    ?
                    .Init(() =>
                    {
                        view.BackAnchorList = new(
                            _ => _
                            .Option(manager.CharacterCreation.LoadPresetOption, () => Arg) // プリセット読み込みボタン
                            .Option(":Done", ChoicesScreen.SaveBackDialog(Save), () => Arg)); // キャラクタークリエイト完了ボタン
                    })
                    .Build();
                };
            }

            private void Save(MMgr manager)
            {
                if (Arg.Arg.Other is CharacterCreationData characterCreationData)
                {
                    // キャラクリ画面から戻ったとき、人形を更新する
                    var figurine = Arg.Arg.TargetObj;
                    KyarakuriFigurineInfo.SetTo(figurine, characterCreationData);
                }

                manager.PopScreen(2);
            }
        }
    }
}
