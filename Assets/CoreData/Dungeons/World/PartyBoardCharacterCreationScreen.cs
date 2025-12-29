using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class PartyBoardCharacterCreationScreen : RogueListuiScreen
    {
        private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
        {
            ScrollSubviewSelector = m => m.CharacterCreation,
        };

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            view.Show(System.Array.Empty<object>(), manager, arg)
                ?
                .Init(() =>
                {
                    view.BackAnchorList = new(
                        _ => _
                        .Option(manager.CharacterCreation.LoadPresetOption) // プリセット読み込みボタン
                        .Option(":Done", ChoicesScreen.SaveBackDialog(Save, null))); // キャラクタークリエイト完了ボタン
                })
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

            manager.PopScreen(2);
        }
    }
}
