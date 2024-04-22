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
    public class MainMenu : IModelsMenu
    {
        private readonly IModelsMenuChoice[] choices;

        public MainMenu(ObjsMenu objsMenu, SkillsMenu skillsMenu, PartyMenu partyMenu)
        {
            choices = new IModelsMenuChoice[]
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

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuThumbnail).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, arg);
            var parent = (StandardMenuRoot)root;
            parent.Stats.SetText(self);
            parent.Stats.SetDungeon(self.Location);
            parent.Stats.Show(true);
        }

        private class Skill : BaseModelsMenuChoice
        {
            public override string Name => ":Skills";

            public IModelsMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Objs : BaseModelsMenuChoice
        {
            public override string Name => ":Items";

            public IModelsMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self);
                root.OpenMenu(nextMenu, self, null, openArg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Ground : BaseModelsMenuChoice
        {
            public override string Name => ":Ground";

            public IModelsMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self, count: 0);
                root.OpenMenu(nextMenu, self, null, openArg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Party : BaseModelsMenuChoice
        {
            public override string Name => ":Party";

            public IModelsMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Log : BaseModelsMenuChoice
        {
            public override string Name => ":Log";

            private IModelsMenu nextMenu = new LogMenu();

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Others : BaseModelsMenuChoice
        {
            public override string Name => ":Others";

            private IModelsMenu nextMenu = new OthersMenu();

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class LogMenu : IModelsMenu
        {
            private IModelsMenuChoice[] choices = new[] { ExitModelsMenuChoice.Instance };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var position = 0f;
                var openArg = new RogueMethodArgument(vector: new Vector2(position, 0f));
                root.Get(DeviceKw.MenuLog).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, openArg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class OthersMenu : IModelsMenu
        {
            private IModelsMenuChoice[] choices = new IModelsMenuChoice[]
            {
                new Save(),
                new GiveUp(),
                new Load(),
                new Quest(),
                new Options(),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
                var inLobby = RogueDevice.Primary.Player.Location == worldInfo.Lobby;
                var openArg = new RogueMethodArgument(count: inLobby ? 1 : 0);
                root.Get(DeviceKw.MenuThumbnail).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, openArg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }

            private class Save : IModelsMenuChoice
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (arg.Count == 1)
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        RogueDevice.Add(DeviceKw.SaveGame, null);

                        // Done するとセーブメニューが消える
                        //root.Done();
                    }
                    else
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    }
                }
            }

            private class GiveUp : BaseModelsMenuChoice
            {
                public override string Name => ":GiveUp";

                private static readonly Choices nextMenu = new Choices();

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.AddInt(DeviceKw.StartTalk, 0);
                    root.AddObject(DeviceKw.AppendText, ":GiveUpMsg");
                    root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                    root.OpenMenuAsDialog(nextMenu, self, user, arg);
                }

                private class Choices : IModelsMenu
                {
                    private static readonly object[] models = new object[]
                    {
                        new GiveUpChoice(),
                        ExitModelsMenuChoice.Instance
                    };

                    public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoiceListPresenter.Instance, models, root, self, user, arg);
                    }
                }

                private class GiveUpChoice : BaseModelsMenuChoice
                {
                    public override string Name => ":Yes";

                    public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.Done();

                        default(IActiveRogueMethodCaller).Defeat(self, user, 0f);
                    }
                }
            }

            private class Load : BaseModelsMenuChoice
            {
                public override string Name => ":Load";

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    RogueDevice.Add(DeviceKw.LoadGame, null);

                    // Done するとロードメニューが消える
                    //root.Done();
                }
            }

            private class Quest : IModelsMenuChoice
            {
                private IModelsMenu nextMenu = new QuestMenu();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (DungeonQuestInfo.TryGetQuest(self, out _)) return ":Quest";
                    else return "<#808080>:Quest";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (DungeonQuestInfo.TryGetQuest(self, out _))
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                    }
                    else
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    }
                }
            }

            private class QuestMenu : IModelsMenu
            {
                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (!DungeonQuestInfo.TryGetQuest(self, out var quest)) throw new RogueException();

                    var summary = (IDungeonQuestMenuView)root.Get(DeviceKw.MenuSummary);
                    summary.OpenView(ChoiceListPresenter.Instance, Spanning<object>.Empty, root, self, null, RogueMethodArgument.Identity);
                    summary.SetQuest(self, quest, false);
                }
            }

            private class Options : BaseModelsMenuChoice
            {
                public override string Name => ":Options";

                private IModelsMenu nextMenu = new OptionsMenu();

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity);
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                }
            }
        }

        private class OptionsMenu : BaseScrollModelsMenu<object>
        {
            private static readonly object[] choices = new object[]
            {
                new OptionsMasterVolume(),
                new OptionsWindowFrameType()
            };

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return choices;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ChoiceListPresenter.Instance.GetItemName(model, root, self, user, arg);
            }

            protected override void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                ChoiceListPresenter.Instance.ActivateItem(model, root, self, user, arg);
            }
        }

        private class OptionsMasterVolume : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "マスター音量";
            }

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var device = (StandardRogueDevice)RogueDevice.Primary;
                return device.Options.MasterVolume;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                var device = (StandardRogueDevice)RogueDevice.Primary;
                device.Options.SetMasterVolume(value);
            }
        }

        private class OptionsWindowFrameType : BaseModelsMenuChoice
        {
            public override string Name => "ウィンドウタイプ";

            private static readonly Menu nextMenu = new Menu();

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, user, RogueMethodArgument.Identity);
            }

            private class Menu : BaseScrollModelsMenu<int>
            {
                private static readonly List<int> models = new();

                protected override Spanning<int> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    if (models.Count != WindowFrameList.Count)
                    {
                        models.Clear();
                        for (int i = 0; i < WindowFrameList.Count; i++)
                        {
                            models.Add(i);
                        }
                    }
                    return models;
                }

                protected override string GetItemName(int index, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return WindowFrameList.GetName(index);
                }

                protected override void ActivateItem(int index, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var device = (StandardRogueDevice)RogueDevice.Primary;
                    device.Options.SetWindowFrame(index, device.Options.WindowFrameColor);

                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.Back();
                }
            }
        }
    }
}
