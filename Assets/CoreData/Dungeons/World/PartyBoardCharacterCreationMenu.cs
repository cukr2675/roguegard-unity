using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : RogueMenuScreen
    {
        private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
        {
            ScrollSubviewSelector = m => (m as IMMgr)?.CharacterCreation,
            BackAnchorList = new(
                _ => _
                .Option(null) // プリセット読み込みボタン（OpenScreen で設定）
                .Option(":Done", ChoicesMenuScreen.SaveBackDialog(Save, null))), // キャラクタークリエイト完了ボタン
        };

        public override void OpenScreen(MMgr manager, MArg arg)
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
                // キャラクリ画面から戻ったとき、そのキャラを更新する
                var character = arg.Arg.TargetObj;
                if (character != null)
                {
                    // 編集キャラ更新
                    character.Main.SetBaseInfoSet(character, characterCreationData.PrimaryInfoSet);
                }
                else
                {
                    // 新規キャラ追加
                    var worldInfo = RogueWorldInfo.GetByCharacter(arg.Self);
                    character = characterCreationData.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                    worldInfo.LobbyMembers.Add(character);
                }
                var info = LobbyMemberList.GetMemberInfo(character);
                info.CharacterCreationData = characterCreationData;
            }

            manager.PopMenuScreen(2);
        }
    }
}
