using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    /// <summary>
    /// メインメニュー（開いてすぐのメニュー）
    /// </summary>
    public class MainMenu : RogueMenuScreen
    {
        private readonly ViewTemplate view = new();

        private readonly ObjsMenu objsMenu;
        private readonly SkillsMenu skillsMenu;
        private readonly PartyMenu partyMenu;
        private readonly LogMenu logMenu = new();
        private readonly OthersMenu othersMenu = new();

        public MainMenu(ObjsMenu objsMenu, SkillsMenu skillsMenu, PartyMenu partyMenu)
        {
            this.objsMenu = objsMenu;
            this.skillsMenu = skillsMenu;
            this.partyMenu = partyMenu;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.ShowTemplate(manager, arg)
                ?
                .Option(":Skills", skillsMenu.Use)
                .Option(":Items", (manager, arg) => manager.PushMenuScreen(objsMenu.Items, arg.Self, null, targetObj: arg.Self))
                .Option(":Ground", (manager, arg) => manager.PushMenuScreen(objsMenu.Ground, arg.Self, null, targetObj: arg.Self))
                .Option(":Party", partyMenu)
                .Option(":Log", logMenu)
                .Option(":Others", othersMenu)
                .Append(objsMenu.Close)
                .Build();
        }

        private class ViewTemplate : MainMenuViewTemplate<MMgr, MArg>
        {
            protected override void ShowSubViews(MMgr manager, MArg arg)
            {
                base.ShowSubViews(manager, arg);

                // ダンジョン名とパーティの名前/HP/MPを表示
                var parent = (MenuController)manager;
                parent.Stats.SetText(arg.Self);
                parent.Stats.SetDungeon(arg.Self.Location);
                parent.Stats.Show();
            }
        }

        private class LogMenu : RogueMenuScreen
        {
            private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
            {
                PrimaryCommandSubViewName = StandardSubViewTable.LongMessageName,
                BackAnchorSubViewName = StandardSubViewTable.BackAnchorName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(manager, arg)
                    ?
                    .Build();
            }
        }

        private class OthersMenu : RogueMenuScreen
        {
            private readonly GiveUpMenu giveUpMenu = new();
            private readonly QuestMenu questMenu = new();
            private readonly OptionsMenu optionsMenu = new();

            private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
                var inLobby = RogueDevice.Primary.Player.Location == worldInfo.Lobby;
                var openArg = new RogueMethodArgument(count: inLobby ? 1 : 0);

                view.ShowTemplate(manager, arg)
                    ?
                    .Option(":Save", (manager, arg) =>
                    {
                        RogueDevice.Add(DeviceKw.SaveGame, null);

                        // Done するとセーブメニューが消える
                        //root.Done();
                    })
                    .Option(":GiveUp", (manager, arg) =>
                    {
                        manager.PushMenuScreen(giveUpMenu, arg);
                    })
                    .Option(":Load", (manager, arg) =>
                    {
                        RogueDevice.Add(DeviceKw.LoadGame, null);

                        // Done するとロードメニューが消える
                        //root.Done();
                    })
                    .Option(":Quest", (manager, arg) =>
                    {
                        if (DungeonQuestInfo.TryGetQuest(arg.Self, out _))
                        {
                            manager.PushMenuScreen(questMenu, arg.Self);
                        }
                    })
                    .Option(":Options", optionsMenu)
                    .Back()
                    .Build();
            }

            private class GiveUpMenu : RogueMenuScreen
            {
                private readonly SpeechBoxViewTemplate<MMgr, MArg> view = new()
                {
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    view.ShowTemplate(":GiveUpMsg", manager, arg)
                        ?.Option(":Yes", (manager, arg) =>
                        {
                            manager.Done();

                            default(IActiveRogueMethodCaller).Defeat(arg.Self, arg.User, 0f);
                        })
                        .Back()
                        .Build();
                }
            }

            private class QuestMenu : RogueMenuScreen
            {
                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    if (!DungeonQuestInfo.TryGetQuest(arg.Self, out var quest)) throw new RogueException();

                    manager.SetQuest(arg.Self, quest, false);
                }
            }
        }

        private class OptionsMenu : RogueMenuScreen
        {
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate("", manager, arg)
                    ?
                    .Append(
                        new object[]
                        {
                            "マスター音量",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) =>
                                {
                                    var device = (StandardRogueDevice)RogueDevice.Primary;
                                    return Mathf.FloorToInt(device.Options.MasterVolume * 100f).ToString();
                                },
                                (manager, arg, valueString) =>
                                {
                                    if (!int.TryParse(valueString, out var value)) { value = 0; }

                                    value = Mathf.Clamp(value, 0, 100);
                                    var device = (StandardRogueDevice)RogueDevice.Primary;
                                    device.Options.SetMasterVolume(value / 100f);

                                    // 音量確認用の効果音を鳴らす
                                    manager.StandardSubViewTable.MessageBox.PlayString("Submit");

                                    return value.ToString();
                                },
                                TMP_InputField.ContentType.IntegerNumber)
                        })

                    .VariableOnce(out var windowTypeScreen, new WindowTypeScreen())
                    .AppendSelectOption("ウィンドウタイプ", windowTypeScreen)

                    .Build();
            }

            private class WindowTypeScreen : RogueMenuScreen
            {
                private readonly List<object> elms = new();

                private readonly ScrollViewTemplate<object, MMgr, MArg> view = new()
                {
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    if (elms.Count != WindowFrameList.Count)
                    {
                        elms.Clear();
                        for (int i = 0; i < WindowFrameList.Count; i++)
                        {
                            elms.Add(new object());
                        }
                    }

                    view.ShowTemplate(elms, manager, arg)
                        ?
                        .ElementNameFrom((element, manager, arg) =>
                        {
                            var index = elms.IndexOf(element);
                            return WindowFrameList.GetName(index);
                        })

                        .OnClickElement((element, manager, arg) =>
                        {
                            var index = elms.IndexOf(element);
                            var device = (StandardRogueDevice)RogueDevice.Primary;
                            device.Options.SetWindowFrame(index, device.Options.WindowFrameColor);

                            manager.Back();
                        })

                        .Build();
                }
            }
        }
    }
}
