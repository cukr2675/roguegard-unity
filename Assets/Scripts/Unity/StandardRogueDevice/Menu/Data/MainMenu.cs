using Lysionium;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RoguegardUnity
{
    /// <summary>
    /// メインメニュー（開いてすぐのメニュー）
    /// </summary>
    public class MainMenu : RogueListuiScreen
    {
        private readonly ViewData view;
        private readonly LogMenu logMenu = new();
        private readonly OthersMenu othersMenu = new();

        public MainMenu(ObjsMenu objsMenu, SkillsMenu skillsMenu, PartyMenu partyMenu)
        {
            view = new ViewData()
            {
                screen = this
            };

            OnOpenScreen += (manager) =>
            {
                view.Show(manager)
                ?
                .Option(
                    name: ":Skills",
                    screen: skillsMenu.Use,
                    args: () => Arg)

                .Option(
                    name: ":Items",
                    onClick: m => m.PushScreen(objsMenu.Items, Arg.Self, null, targetObj: Arg.Self))

                .Option(
                    name: ":Ground",
                    onClick: m => m.PushScreen(objsMenu.Ground, Arg.Self, null, targetObj: Arg.Self))

                .Option(
                    name: ":Party",
                    screen: partyMenu,
                    args: () => Arg)

                .Option(
                    name: ":Log",
                    screen: logMenu,
                    args: () => Arg)

                .Option(
                    name: ":Others",
                    screen: othersMenu,
                    args: () => Arg)

                .Option(objsMenu.Close, () => Arg)

                .Build();
            };
        }

        private class ViewData : MainMenuViewData<MMgr>
        {
            public MainMenu screen;

            protected override void ShowSubviews(MMgr manager)
            {
                base.ShowSubviews(manager);

                // ダンジョン名とパーティの名前/HP/MPを表示
                var parent = (MenuController)manager;
                parent.Stats.SetText(screen.Arg.Self);
                parent.Stats.SetDungeon(screen.Arg.Self.Location);
                parent.Stats.Show();
            }
        }

        private class LogMenu : RogueListuiScreen
        {
            private readonly MainMenuViewData<MMgr> view = new()
            {
                PrimaryCommandSubviewSelector = null,
                BackAnchorSubviewSelector = m => m.BackAnchor,
            };

            public LogMenu()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(manager)
                    ?
                    .Build();

                    manager.LongMessage.Show();
                };
            }
        }

        private class OthersMenu : RogueListuiScreen
        {
            private readonly GiveUpMenu giveUpMenu = new();
            private readonly QuestMenu questMenu = new();
            private readonly OptionsMenu optionsMenu = new();

            private readonly MainMenuViewData<MMgr> view = new()
            {
            };

            public OthersMenu()
            {
                OnOpenScreen += (manager) =>
                {
                    var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
                    var inLobby = RogueDevice.Primary.Player.Location == worldInfo.Lobby;
                    var openArg = new RogueMethodArgument(count: inLobby ? 1 : 0);

                    view.Show(manager)
                    ?
                    .Option(":Save", (manager) =>
                    {
                        RogueDevice.Add(DeviceKw.SaveGame, null);

                        // Done するとセーブメニューが消える
                        //root.Done();
                    })
                    .Option(":GiveUp", (manager) =>
                    {
                        manager.PushScreen(giveUpMenu, Arg);
                    })
                    .Option(":Load", (manager) =>
                    {
                        RogueDevice.Add(DeviceKw.LoadGame, null);

                        // Done するとロードメニューが消える
                        //root.Done();
                    })
                    .Option(":Quest", (manager) =>
                    {
                        if (DungeonQuestInfo.TryGetQuest(Arg.Self, out _))
                        {
                            manager.PushScreen(questMenu, Arg.Self);
                        }
                    })
                    .Option(":Options", optionsMenu, () => Arg)
                    .Back()
                    .Build();
                };
            }

            private class GiveUpMenu : RogueListuiScreen
            {
                private readonly SpeechBoxViewData<MMgr> view = new()
                {
                };

                public GiveUpMenu()
                {
                    OnOpenScreen += (manager) =>
                    {
                        view.Show(":GiveUpMsg", manager)
                        ?.Option(":Yes", (manager) =>
                        {
                            manager.Done();

                            default(IActiveRogueMethodCaller).Defeat(Arg.Self, Arg.User, 0f);
                        })
                        .Back()
                        .Build();
                    };
                }
            }

            private class QuestMenu : RogueListuiScreen
            {
                public QuestMenu()
                {
                    OnOpenScreen += (manager) =>
                    {
                        if (!DungeonQuestInfo.TryGetQuest(Arg.Self, out var quest)) throw new System.InvalidOperationException();

                        manager.Summary.SetQuest(Arg.Self, quest, false, manager);
                        manager.Summary.Show();
                    };
                }
            }
        }

        private class OptionsMenu : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public OptionsMenu()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show("", manager)
                    ?
                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", "マスター音量"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ =>
                            {
                                var device = (StandardRogueDevice)RogueDevice.Primary;
                                return Mathf.FloorToInt(device.Options.MasterVolume * 100f).ToString();
                            },
                            valueString =>
                            {
                                if (!int.TryParse(valueString, out var value)) { value = 0; }

                                value = Mathf.Clamp(value, 0, 100);
                                var device = (StandardRogueDevice)RogueDevice.Primary;
                                device.Options.SetMasterVolume(value / 100f);

                                // 音量確認用の効果音を鳴らす
                                ((Lysionium.Views.Subview)manager.MessageBox).PlayEvtfxString("Submit");

                                return value.ToString();
                            },
                            TMP_InputField.ContentType.IntegerNumber))))

                    .VarOnce(out var windowTypeScreen, new WindowTypeScreen())
                    .Tail.Option("ウィンドウタイプ", windowTypeScreen, () => Arg)

                    .Build();
                };
            }

            private class WindowTypeScreen : RogueListuiScreen
            {
                private readonly List<object> indexList = new();

                private readonly ScrollMenuViewData<object, MMgr> view = new()
                {
                };

                public WindowTypeScreen()
                {
                    OnOpenScreen += (manager) =>
                    {
                        if (indexList.Count != WindowFrameList.Count)
                        {
                            indexList.Clear();
                            for (int i = 0; i < WindowFrameList.Count; i++)
                            {
                                indexList.Add(new object());
                            }
                        }

                        view.Show(indexList, manager)
                        ?
                        .NameFrom((item, manager) =>
                        {
                            var index = indexList.IndexOf(item);
                            return WindowFrameList.GetName(index);
                        })

                        .OnClick((item, manager) =>
                        {
                            var index = indexList.IndexOf(item);
                            var device = (StandardRogueDevice)RogueDevice.Primary;
                            device.Options.SetWindowFrame(index, device.Options.WindowFrameColor);

                            manager.PopScreen();
                        })

                        .Build();
                    };
                }
            }
        }
    }
}
