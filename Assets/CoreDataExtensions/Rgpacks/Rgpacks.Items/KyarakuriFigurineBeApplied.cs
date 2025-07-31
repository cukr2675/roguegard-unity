using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            private readonly VariableWidgetsViewData<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(System.Array.Empty<object>(), manager, arg)
                    ?
                    .Tail(
                        new object[]
                        {
                            "アセットID",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => NamingEffect.Get(arg.Arg.TargetObj)?.Naming,
                                (manager, arg, value) =>
                                {
                                    var figurine = arg.Arg.TargetObj;
                                    default(IActiveRogueMethodCaller).Affect(figurine, 1f, NamingEffect.Callback);
                                    return NamingEffect.Get(figurine).Naming = value;
                                })
                        })

                    .VarOnce(out var nextMenu, new EditMenu())
                    .Tail(SelectOption.Create<MMgr, MArg>(
                        "キャラクリ設定",
                        (manager, arg) =>
                        {
                            var figurine = arg.Arg.TargetObj;
                            var characterCreationData = new CharacterCreationData(KyarakuriFigurineInfo.Get(figurine));
                            manager.PushMenuScreen(nextMenu, arg.Self, targetObj: figurine, other: characterCreationData);
                        }))

                    .Build();
            }
        }

        private class EditMenu : RogueMenuScreen
        {
            private readonly ScrollViewData<object, MMgr, MArg> view = new()
            {
                ScrollSubviewName = RoguegardSubviews.CharacterCreation,
                BackAnchorList = new()
                {
                    // プリセット読み込みボタン（OpenScreen で設定）
                    null,
                    
                    // キャラクタークリエイト完了ボタン
                    SelectOption.Create<MMgr, MArg>(
                        ":Done", ChoicesMenuScreen.SaveBackDialog(Save, null))
                },
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                // プリセット読み込みボタンを設定する
                var characterCreation = RoguegardSubviews.GetCharacterCreation(manager);
                view.BackAnchorList[0] = characterCreation.LoadPresetOption;

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
