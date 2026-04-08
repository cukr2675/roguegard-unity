using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class PartyBoardCharacterCreationScreen : RogueListuiScreen
    {
        private readonly CharacterCreationViewData view = new()
        {
        };

        public PartyBoardCharacterCreationScreen()
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
                        .Option(":Done", ChoicesScreen.SaveBackDialog(Save, null), () => Arg)); // キャラクタークリエイト完了ボタン
                })
                .Build();
            };
        }

        private void Save(MMgr manager)
        {
            if (Arg.Arg.Other is CharacterCreationData characterCreationData)
            {
                // キャラクリ画面から戻ったとき、そのキャラを更新する
                var character = Arg.Arg.TargetObj;
                if (character != null)
                {
                    // 編集キャラ更新
                    character.Main.SetBaseInfoSet(character, characterCreationData.PrimaryInfoSet);
                }
                else
                {
                    // 新規キャラ追加
                    var worldInfo = RogueWorldInfo.GetByCharacter(Arg.Self);
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
