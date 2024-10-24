using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            view.Show(manager, arg)
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

        private class ViewTemplate : MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg>
        {
            protected override void ShowSubViews(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                base.ShowSubViews(manager, arg);

                var parent = (MenuController)manager;
                parent.Stats.SetText(arg.Self);
                parent.Stats.SetDungeon(arg.Self.Location);
                parent.Stats.Show(true);
            }
        }

        private class LogMenu : RogueMenuScreen
        {
            private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.Show("", manager, arg)
                    ?.Build();
            }
        }

        private class OthersMenu : RogueMenuScreen
        {
            private readonly GiveUpMenu giveUpMenu = new();
            private readonly QuestMenu questMenu = new();
            private readonly OptionsMenu optionsMenu = new();

            private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
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
                    .Exit()
                    .Build();
            }

            private class GiveUpMenu : RogueMenuScreen
            {
                private readonly SpeechBoxViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
                {
                };

                public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
                {
                    view.Show(":GiveUpMsg", manager, arg)
                        ?.Option(":Yes", (manager, arg) =>
                        {
                            manager.Done();

                            default(IActiveRogueMethodCaller).Defeat(arg.Self, arg.User, 0f);
                        })
                        .Exit()
                        .Build();
                }
            }

            private class QuestMenu : RogueMenuScreen
            {
                private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
                {
                };

                public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
                {
                    if (!DungeonQuestInfo.TryGetQuest(arg.Self, out var quest)) throw new RogueException();

                    view.Show(quest.ToString(), manager, arg)
                        ?.Build();
                }
            }
        }

        private class OptionsMenu : RogueMenuScreen
        {
            //private static readonly object[] selectOptions = new object[]
            //{
            //    new OptionsMasterVolume(),
            //    new OptionsWindowFrameType()
            //};

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                throw new System.NotImplementedException();
            }

            //protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            //{
            //    return selectOptions;
            //}

            //protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            //{
            //    return SelectOptionPresenter.Instance.GetItemName(element, manager, self, user, arg);
            //}

            //protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            //{
            //    SelectOptionPresenter.Instance.ActivateItem(element, manager, self, user, arg);
            //}
        }

        //private class OptionsMasterVolume : IOptionsMenuSlider
        //{
        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return "マスター音量";
        //    }

        //    public float GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var device = (StandardRogueDevice)RogueDevice.Primary;
        //        return device.Options.MasterVolume;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
        //    {
        //        var device = (StandardRogueDevice)RogueDevice.Primary;
        //        device.Options.SetMasterVolume(value);
        //    }
        //}

        //private class OptionsWindowFrameType : BaseListMenuSelectOption
        //{
        //    public override string Name => "ウィンドウタイプ";

        //    private static readonly Menu nextMenu = new Menu();

        //    public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
        //        manager.OpenMenu(nextMenu, self, user, RogueMethodArgument.Identity);
        //    }

        //    private class Menu : BaseScrollListMenu<int>
        //    {
        //        private static readonly List<int> elms = new();

        //        protected override Spanning<int> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            if (elms.Count != WindowFrameList.Count)
        //            {
        //                elms.Clear();
        //                for (int i = 0; i < WindowFrameList.Count; i++)
        //                {
        //                    elms.Add(i);
        //                }
        //            }
        //            return elms;
        //        }

        //        protected override string GetItemName(int index, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            return WindowFrameList.GetName(index);
        //        }

        //        protected override void ActivateItem(int index, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            var device = (StandardRogueDevice)RogueDevice.Primary;
        //            device.Options.SetWindowFrame(index, device.Options.WindowFrameColor);

        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
        //            manager.Back();
        //        }
        //    }
        //}
    }
}
