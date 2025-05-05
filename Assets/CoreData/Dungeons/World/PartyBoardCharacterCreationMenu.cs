using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : RogueMenuScreen
    {
        private readonly ScrollViewTemplate<object, MMgr, MArg> view = new()
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

            view.ShowTemplate(System.Array.Empty<object>(), manager, arg)
                ?
                .Build();
        }

        private static void Save(MMgr manager, MArg arg)
        {
            if (arg.Arg.Other is CharacterCreationDataBuilder builder)
            {
                // キャラクリ画面から戻ったとき、そのキャラを更新する
                var character = arg.Arg.TargetObj;
                if (character != null)
                {
                    // 編集キャラ更新
                    character.Main.SetBaseInfoSet(character, builder.PrimaryInfoSet);
                }
                else
                {
                    // 新規キャラ追加
                    var worldInfo = RogueWorldInfo.GetByCharacter(arg.Self);
                    character = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                    worldInfo.LobbyMembers.Add(character);
                }
                var info = LobbyMemberList.GetMemberInfo(character);
                info.CharacterCreationData = builder;
            }

            manager.PopMenuScreen(2);
        }
    }
}
