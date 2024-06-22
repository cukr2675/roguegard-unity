using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    /// <summary>
    /// メインメニュー（開いてすぐのメニュー）
    /// </summary>
    public class MainMenu : IListMenu
    {
        private readonly IListMenuSelectOption[] selectOptions;

        public MainMenu(ObjsMenu objsMenu, SkillsMenu skillsMenu, PartyMenu partyMenu)
        {
            selectOptions = new IListMenuSelectOption[]
            {
                new Skill() { nextMenu = skillsMenu.Use },
                new Objs() { nextMenu = objsMenu.Items },
                new Ground() { nextMenu = objsMenu.Ground },
                new Party() { nextMenu = partyMenu },
                new Log(),
                new Others(),
                objsMenu.Close,
            };
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.GetView(DeviceKw.MenuThumbnail).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
            var parent = (StandardMenuManager)manager;
            parent.Stats.SetText(self);
            parent.Stats.SetDungeon(self.Location);
            parent.Stats.Show(true);
        }

        private class Skill : BaseListMenuSelectOption
        {
            public override string Name => ":Skills";

            public IListMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Objs : BaseListMenuSelectOption
        {
            public override string Name => ":Items";

            public IListMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self);
                manager.OpenMenu(nextMenu, self, null, openArg);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Ground : BaseListMenuSelectOption
        {
            public override string Name => ":Ground";

            public IListMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self, count: 0);
                manager.OpenMenu(nextMenu, self, null, openArg);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Party : BaseListMenuSelectOption
        {
            public override string Name => ":Party";

            public IListMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Log : BaseListMenuSelectOption
        {
            public override string Name => ":Log";

            private IListMenu nextMenu = new LogMenu();

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Others : BaseListMenuSelectOption
        {
            public override string Name => ":Others";

            private IListMenu nextMenu = new OthersMenu();

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class LogMenu : IListMenu
        {
            private IListMenuSelectOption[] selectOptions = new[] { ExitListMenuSelectOption.Instance };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var position = 0f;
                var openArg = new RogueMethodArgument(vector: new Vector2(position, 0f));
                manager.GetView(DeviceKw.MenuLog).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, openArg);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class OthersMenu : IListMenu
        {
            private IListMenuSelectOption[] selectOptions = new IListMenuSelectOption[]
            {
                new Save(),
                new GiveUp(),
                new Load(),
                new Quest(),
                new Options(),
                ExitListMenuSelectOption.Instance
            };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
                var inLobby = RogueDevice.Primary.Player.Location == worldInfo.Lobby;
                var openArg = new RogueMethodArgument(count: inLobby ? 1 : 0);
                manager.GetView(DeviceKw.MenuThumbnail).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, openArg);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }

            private class Save : IListMenuSelectOption
            {
                public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    // ロビー以外ではセーブをグレーアウトする
                    if (arg.Count == 1)
                    {
                        return ":Save";
                    }
                    else
                    {
                        return "<#808080>:Save";
                    }
                }

                public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (arg.Count == 1)
                    {
                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        RogueDevice.Add(DeviceKw.SaveGame, null);

                        // Done するとセーブメニューが消える
                        //root.Done();
                    }
                    else
                    {
                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    }
                }
            }

            private class GiveUp : BaseListMenuSelectOption
            {
                public override string Name => ":GiveUp";

                private static readonly SelectOptions nextMenu = new SelectOptions();

                public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.AddInt(DeviceKw.StartTalk, 0);
                    manager.AddObject(DeviceKw.AppendText, ":GiveUpMsg");
                    manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                    manager.OpenMenuAsDialog(nextMenu, self, user, arg);
                }

                private class SelectOptions : IListMenu
                {
                    private static readonly object[] elms = new object[]
                    {
                        new GiveUpSelectOption(),
                        ExitListMenuSelectOption.Instance
                    };

                    public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        manager.GetView(DeviceKw.MenuTalkSelect).OpenView(SelectOptionPresenter.Instance, elms, manager, self, user, arg);
                    }
                }

                private class GiveUpSelectOption : BaseListMenuSelectOption
                {
                    public override string Name => ":Yes";

                    public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        manager.Done();

                        default(IActiveRogueMethodCaller).Defeat(self, user, 0f);
                    }
                }
            }

            private class Load : BaseListMenuSelectOption
            {
                public override string Name => ":Load";

                public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    RogueDevice.Add(DeviceKw.LoadGame, null);

                    // Done するとロードメニューが消える
                    //root.Done();
                }
            }

            private class Quest : IListMenuSelectOption
            {
                private IListMenu nextMenu = new QuestMenu();

                public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (DungeonQuestInfo.TryGetQuest(self, out _)) return ":Quest";
                    else return "<#808080>:Quest";
                }

                public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (DungeonQuestInfo.TryGetQuest(self, out _))
                    {
                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                    }
                    else
                    {
                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    }
                }
            }

            private class QuestMenu : IListMenu
            {
                public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (!DungeonQuestInfo.TryGetQuest(self, out var quest)) throw new RogueException();

                    var summary = (IDungeonQuestMenuView)manager.GetView(DeviceKw.MenuSummary);
                    summary.OpenView(SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, self, null, RogueMethodArgument.Identity);
                    summary.SetQuest(self, quest, false);
                }
            }

            private class Options : BaseListMenuSelectOption
            {
                public override string Name => ":Options";

                private IListMenu nextMenu = new OptionsMenu();

                public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    manager.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                }
            }
        }

        private class OptionsMenu : BaseScrollListMenu<object>
        {
            private static readonly object[] selectOptions = new object[]
            {
                new OptionsMasterVolume(),
                new OptionsWindowFrameType()
            };

            protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return selectOptions;
            }

            protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return SelectOptionPresenter.Instance.GetItemName(element, manager, self, user, arg);
            }

            protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                SelectOptionPresenter.Instance.ActivateItem(element, manager, self, user, arg);
            }
        }

        private class OptionsMasterVolume : IOptionsMenuSlider
        {
            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "マスター音量";
            }

            public float GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var device = (StandardRogueDevice)RogueDevice.Primary;
                return device.Options.MasterVolume;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                var device = (StandardRogueDevice)RogueDevice.Primary;
                device.Options.SetMasterVolume(value);
            }
        }

        private class OptionsWindowFrameType : BaseListMenuSelectOption
        {
            public override string Name => "ウィンドウタイプ";

            private static readonly Menu nextMenu = new Menu();

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.OpenMenu(nextMenu, self, user, RogueMethodArgument.Identity);
            }

            private class Menu : BaseScrollListMenu<int>
            {
                private static readonly List<int> elms = new();

                protected override Spanning<int> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (elms.Count != WindowFrameList.Count)
                    {
                        elms.Clear();
                        for (int i = 0; i < WindowFrameList.Count; i++)
                        {
                            elms.Add(i);
                        }
                    }
                    return elms;
                }

                protected override string GetItemName(int index, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return WindowFrameList.GetName(index);
                }

                protected override void ActivateItem(int index, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var device = (StandardRogueDevice)RogueDevice.Primary;
                    device.Options.SetWindowFrame(index, device.Options.WindowFrameColor);

                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.Back();
                }
            }
        }
    }
}
