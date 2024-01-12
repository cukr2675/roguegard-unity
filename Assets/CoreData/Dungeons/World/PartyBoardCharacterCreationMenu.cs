using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PartyBoardCharacterCreationMenu : IModelsMenu
    {
        private readonly object[] models = new object[] { new OpenExitDialog() };

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var builder = (CharacterCreationDataBuilder)arg.Other;
            root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, self, user, new(targetObj: arg.TargetObj, other: builder));
        }

        private class OpenExitDialog : IModelsMenuChoice
        {
            private static readonly ExitDialog nextMenu = new ExitDialog();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "<";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddInt(DeviceKw.StartTalk, 0);
                root.AddObject(DeviceKw.AppendText, "編集内容を保存しますか？");
                root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                root.OpenMenuAsDialog(nextMenu, self, user, arg, arg);
            }
        }

        private class ExitDialog : IModelsMenu
        {
            private readonly object[] models = new object[]
            {
                new ActionModelsMenuChoice("上書き保存", Save),
                new ActionModelsMenuChoice("保存しない", NotSave),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
            }

            private static void Save(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Other is CharacterCreationDataBuilder builder)
                {
                    // キャラクリ画面から戻ったとき、そのキャラを更新する
                    var character = arg.TargetObj;
                    if (character != null)
                    {
                        // 編集キャラ更新
                        character.Main.SetBaseInfoSet(character, builder.PrimaryInfoSet);
                    }
                    else
                    {
                        // 新規キャラ追加
                        var world = RogueWorld.GetWorld(player);
                        character = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                        LobbyMembers.Add(character, world);
                    }
                    var info = LobbyMembers.GetMemberInfo(character);
                    info.CharacterCreationData = builder;
                }

                root.Back();
                root.Back();
            }

            private static void NotSave(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 何もせず閉じる
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                root.Back();
            }
        }
    }
}
