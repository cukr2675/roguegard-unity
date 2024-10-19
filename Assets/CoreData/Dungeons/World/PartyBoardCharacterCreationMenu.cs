using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : RogueMenuScreen
    {
        private readonly ListMenuSelectOption<RogueMenuManager, ReadOnlyMenuArg> exit
            = ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("<", Save);

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            manager.OpenCharacterCreationView(exit, arg);
        }

        private static void Save(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

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

            manager.HandleClickBack();
            manager.HandleClickBack();
        }
    }
}
