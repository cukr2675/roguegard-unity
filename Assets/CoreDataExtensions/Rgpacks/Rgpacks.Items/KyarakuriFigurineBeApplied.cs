using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class KyarakuriFigurineBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            var characterCreationInfo = KyarakuriFigurineInfo.Get(self);
            if (characterCreationInfo == null)
            {
                KyarakuriFigurineInfo.SetTo(self, RoguegardSettings.CharacterCreationDatabase.LoadPreset(0));
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : RogueMenuScreen
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
                        (manager, arg, value) =>
                        {
                            var figurine = arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(figurine, 1f, NamingEffect.Callback);
                            return NamingEffect.Get(figurine).Naming = value;
                        }))

                    .VarOnce(out var nextMenu, new EditMenu())
                    .Tail.Option("キャラクリ設定", (manager, arg) =>
                    {
                        var figurine = arg.Arg.TargetObj;
                        var characterCreationData = new CharacterCreationData(KyarakuriFigurineInfo.Get(figurine));
                        manager.PushMenuScreen(nextMenu, arg.Self, targetObj: figurine, other: characterCreationData);
                    })

                    .Build();
            }
        }

        private class EditMenu : RogueMenuScreen
        {
            private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
            {
                ScrollSubviewSelector = m => m.CharacterCreation,
                BackAnchorList = new(
                    _ => _
                    .Option(null) // プリセット読み込みボタン（OpenScreen で設定）
                    .Option(":Done", ChoicesMenuScreen.SaveBackDialog(Save))), // キャラクタークリエイト完了ボタン
            };

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                // プリセット読み込みボタンを設定する
                view.BackAnchorList[0] = manager.CharacterCreation.LoadPresetOption;

                view.Show(System.Array.Empty<object>(), manager, arg)
                    ?
                    .Build();
            }

            private static void Save(MMgr manager, MArg arg)
            {
                if (arg.Arg.Other is CharacterCreationData characterCreationData)
                {
                    // キャラクリ画面から戻ったとき、人形を更新する
                    var figurine = arg.Arg.TargetObj;
                    KyarakuriFigurineInfo.SetTo(figurine, characterCreationData);
                }

                manager.PopMenuScreen(2);
            }
        }
    }
}
