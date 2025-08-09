using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Lysionium;
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
        private readonly ViewData view = new();

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
            view.Show(manager, arg)
                ?
                .Option(":Skills", skillsMenu.Use)
                .Option(":Items", (manager, arg) => manager.PushMenuScreen(objsMenu.Items, arg.Self, null, targetObj: arg.Self))
                .Option(":Ground", (manager, arg) => manager.PushMenuScreen(objsMenu.Ground, arg.Self, null, targetObj: arg.Self))
                .Option(":Party", partyMenu)
                .Option(":Log", logMenu)
                .Option(":Others", othersMenu)
                .Tail(objsMenu.Close)
                .Build();
        }

        private class ViewData : MainMenuViewData<MMgr, MArg>
        {
            protected override void ShowSubviews(MMgr manager, MArg arg)
            {
                base.ShowSubviews(manager, arg);

                // ダンジョン名とパーティの名前/HP/MPを表示
                var parent = (MenuController)manager;
                parent.Stats.SetText(arg.Self);
                parent.Stats.SetDungeon(arg.Self.Location);
                parent.Stats.Show();
            }
        }

        private class LogMenu : RogueMenuScreen
        {
            private readonly MainMenuViewData<MMgr, MArg> view = new()
            {
                PrimaryCommandSubviewName = StandardSubviewTable.LongMessageName,
                BackAnchorSubviewName = StandardSubviewTable.BackAnchorName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(manager, arg)
                    ?
                    .Build();
            }
        }

        private class OthersMenu : RogueMenuScreen
        {
            private readonly GiveUpMenu giveUpMenu = new();
            private readonly QuestMenu questMenu = new();
            private readonly OptionsMenu optionsMenu = new();

            private readonly MainMenuViewData<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
                var inLobby = RogueDevice.Primary.Player.Location == worldInfo.Lobby;
                var openArg = new RogueMethodArgument(count: inLobby ? 1 : 0);

                view.Show(manager, arg)
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
                private readonly SpeechBoxViewData<MMgr, MArg> view = new()
                {
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    view.Show(":GiveUpMsg", manager, arg)
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
                    if (!DungeonQuestInfo.TryGetQuest(arg.Self, out var quest)) throw new System.InvalidOperationException();

                    var summary = RoguegardSubviews.GetSummary(manager);
                    summary.SetQuest(arg.Self, quest, false, manager);
                    summary.Show();
                }
            }
        }

        private class OptionsMenu : RogueMenuScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewName = StandardSubviewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show("", manager, arg)
                    ?
                    .Tail(StackViewWidget.CreateOption(
                        ("1*", "マスター音量"),
                        ("1*", InputFieldViewWidget.CreateOption<MMgr, MArg>(
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
                                manager.StandardSubviewTable.MessageBox.PlayString("Submit");

                                return value.ToString();
                            },
                            TMP_InputField.ContentType.IntegerNumber))))

                    .VarOnce(out var windowTypeScreen, new WindowTypeScreen())
                    .Option("ウィンドウタイプ", windowTypeScreen)

                    .Build();
            }

            private class WindowTypeScreen : RogueMenuScreen
            {
                private readonly List<object> indexList = new();

                private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
                {
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    if (indexList.Count != WindowFrameList.Count)
                    {
                        indexList.Clear();
                        for (int i = 0; i < WindowFrameList.Count; i++)
                        {
                            indexList.Add(new object());
                        }
                    }

                    view.Show(indexList, manager, arg)
                        ?
                        .NameFrom((item, manager, arg) =>
                        {
                            var index = indexList.IndexOf(item);
                            return WindowFrameList.GetName(index);
                        })

                        .OnClick((item, manager, arg) =>
                        {
                            var index = indexList.IndexOf(item);
                            var device = (StandardRogueDevice)RogueDevice.Primary;
                            device.Options.SetWindowFrame(index, device.Options.WindowFrameColor);

                            manager.PopMenuScreen();
                        })

                        .Build();
                }
            }
        }
    }
}
