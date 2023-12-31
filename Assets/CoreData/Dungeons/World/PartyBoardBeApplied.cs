using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class PartyBoardBeApplied : ReferableScript, IApplyRogueMethod
    {
        private PartyBoardBeApplied() { }

        IRogueMethodTarget ISkillDescription.Target => null;
        IRogueMethodRange ISkillDescription.Range => null;
        int ISkillDescription.RequiredMP => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        private static readonly RogueMenu rogueMenu = new RogueMenu();

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddMenu(rogueMenu, user, null, RogueMethodArgument.Identity);
                return true;
            }
            else
            {
                return false;
            }
        }

        int ISkillDescription.GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        public class RogueMenu : IModelsMenu
        {
            private static readonly ItemController itemController = new ItemController();
            private static readonly List<object> models = new List<object>();

            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is CharacterCreationDataBuilder builder)
                {
                    // キャラクリ画面から戻ったとき、そのキャラを更新する
                    if (!builder.TryGetGrowingInfoSet(builder.Race.Option, builder.Race.Gender, out var newInfoSet)) throw new RogueException();

                    var character = arg.TargetObj;
                    if (character != null)
                    {
                        // 編集キャラ更新
                        character.Main.SetBaseInfoSet(character, newInfoSet);
                    }
                    else
                    {
                        // 新規キャラ追加
                        var world = RogueWorld.GetWorld(player);
                        character = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                        LobbyMembers.Add(character, world);
                    }
                    LobbyMembers.SetCharacterCreationDataBuilder(character, builder);
                }

                var lobbyMembers = LobbyMembers.GetMembersByCharacter(player);
                models.Clear();
                for (int i = 0; i < lobbyMembers.Count; i++)
                {
                    if (lobbyMembers[i] == null) continue;

                    models.Add(lobbyMembers[i]);
                }
                models.Add(null);

                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(itemController, models, root, player, null, RogueMethodArgument.Identity);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            private class ItemController : IModelsMenuItemController
            {
                private static readonly CommandMenu nextMenu = new CommandMenu();
                private static readonly NewMenu newMenu = new NewMenu();

                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (model == null) return "+ 追加";
                    else return ((RogueObj)model).GetName();
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (model == null)
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                        var openArg = new RogueMethodArgument(other: builder);
                        root.OpenMenu(newMenu, self, user, openArg, openArg);
                    }
                    else
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        var openArg = new RogueMethodArgument(targetObj: (RogueObj)model);
                        root.OpenMenuAsDialog(nextMenu, self, user, openArg, RogueMethodArgument.Identity);
                    }
                }
            }

            private class CommandMenu : IModelsMenu
            {
                private static readonly object[] models = new object[]
                {
                    new Change(),
                    new Edit(),
                    ExitModelsMenuChoice.Instance
                };

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var newPlayer = arg.TargetObj;
                    var openArg = new RogueMethodArgument(targetObj: newPlayer);
                    root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, null, openArg);
                }
            }

            private class Change : IModelsMenuChoice
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "交代";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var newPlayer = arg.TargetObj;

                    // 空間移動
                    var location = self.Location;
                    var position = self.Position;
                    SpaceUtility.TryLocate(self, null);
                    SpaceUtility.TryLocate(newPlayer, location, position);
                    newPlayer.Main.Stats.Direction = RogueDirection.Down;

                    // パーティ移動
                    var party = self.Main.Stats.Party;
                    self.Main.Stats.UnassignParty(self, party);
                    newPlayer.Main.Stats.TryAssignParty(newPlayer, party);

                    RogueDevice.Primary.AddObject(DeviceKw.ChangePlayer, newPlayer);

                    root.Done();
                }
            }

            private class Edit : IModelsMenuChoice
            {
                private static readonly Menu nextMenu = new Menu();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "編集";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var character = arg.TargetObj;
                    var builder = LobbyMembers.GetCharacterCreationDataBuilder(character);
                    builder = new CharacterCreationDataBuilder(builder);

                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.OpenMenu(nextMenu, self, user, new(targetObj: character, other: builder), new(targetObj: character, other: builder));
                }

                private class Menu : IModelsMenu
                {
                    private readonly object[] models = new object[] { new OpenExitDialog() };

                    public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        var builder = (CharacterCreationDataBuilder)arg.Other;
                        root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, self, user, new(targetObj: arg.TargetObj, other: builder));
                    }
                }
            }

            private class NewMenu : IModelsMenu
            {
                private readonly object[] models = new object[] { ExitModelsMenuChoice.Instance };

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (CharacterCreationDataBuilder)arg.Other;
                    root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, self, user, new(other: builder));
                }
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
                    new SaveChoice(),
                    new NotSaveChoice(),
                    ExitModelsMenuChoice.Instance
                };

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
                }

                private class SaveChoice : IModelsMenuChoice
                {
                    public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        return "上書き保存";
                    }

                    public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.Back();
                        root.Back();
                    }
                }

                private class NotSaveChoice : IModelsMenuChoice
                {
                    public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        return "保存しない";
                    }

                    public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        // 編集を取り消す
                        var character = arg.TargetObj;
                        var baseBuilder = LobbyMembers.GetCharacterCreationDataBuilder(character);
                        var builder = (CharacterCreationDataBuilder)arg.Other;
                        builder.Set(baseBuilder);

                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.Back();
                        root.Back();
                    }
                }
            }
        }
    }
}
