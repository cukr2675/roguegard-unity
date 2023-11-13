using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// メインメニュー（開いてすぐのメニュー）
    /// </summary>
    public class MainMenu : IModelsMenu
    {
        private readonly IModelsMenuChoice[] choices;

        public MainMenu(CaptionWindow captionWindow, ObjsMenu objsMenu, SkillsMenu skillsMenu, PartyMenu partyMenu)
        {
            choices = new IModelsMenuChoice[]
            {
                new Skill() { captionWindow = captionWindow, nextMenu = skillsMenu.Use },
                new Objs() { captionWindow = captionWindow, nextMenu = objsMenu.Items },
                new Ground() { captionWindow = captionWindow, nextMenu = objsMenu.Ground },
                new Party() { captionWindow = captionWindow, nextMenu = partyMenu },
                new Log(),
                new Others(),
                objsMenu.Close,
            };
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuThumbnail).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
            var parent = (MenuController)root;
            parent.Stats.SetText(self);
            parent.Stats.SetDungeon(self.Location);
            parent.Stats.Show(true);
        }

        private class Skill : IModelsMenuChoice
        {
            public CaptionWindow captionWindow;
            public IModelsMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Skill";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                captionWindow.Show(true);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Objs : IModelsMenuChoice
        {
            public CaptionWindow captionWindow;
            public IModelsMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Items";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self);
                root.OpenMenu(nextMenu, self, null, openArg, RogueMethodArgument.Identity);
                captionWindow.Show(true);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Ground : IModelsMenuChoice
        {
            public CaptionWindow captionWindow;
            public IModelsMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Ground";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: self, count: 0);
                root.OpenMenu(nextMenu, self, null, openArg, RogueMethodArgument.Identity);
                captionWindow.Show(true);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Party : IModelsMenuChoice
        {
            public CaptionWindow captionWindow;
            public IModelsMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Party";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                captionWindow.Show(true);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Log : IModelsMenuChoice
        {
            private IModelsMenu nextMenu = new LogMenu();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Log";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }

        private class Others : IModelsMenuChoice
        {
            private IModelsMenu nextMenu = new OthersMenu();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Others";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
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
                root.Get(DeviceKw.MenuLog).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, openArg);
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
                new Options(),
                ExitModelsMenuChoice.Instance
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var lobby = RogueWorld.GetLobbyByCharacter(RogueDevice.Primary.Player);
                var inLobby = RogueDevice.Primary.Player.Location == lobby;
                var openArg = new RogueMethodArgument(count: inLobby ? 1 : 0);
                root.Get(DeviceKw.MenuThumbnail).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, openArg);
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

            private class GiveUp : IModelsMenuChoice
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => ":GiveUp";

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.AddInt(DeviceKw.StartTalk, 0);
                    root.AddObject(DeviceKw.AppendText, ":GiveUpMsg");
                }
            }

            private class Load : IModelsMenuChoice
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => ":Load";

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    RogueDevice.Add(DeviceKw.LoadGame, null);

                    // Done するとロードメニューが消える
                    //root.Done();
                }
            }

            private class Options : IModelsMenuChoice
            {
                private IModelsMenu nextMenu = new OptionsMenu();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => ":Options";

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.OpenMenu(nextMenu, self, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                }
            }
        }

        private class OptionsMenu : IModelsMenu
        {
            private IModelsMenuChoice[] choices = new[] { ExitModelsMenuChoice.Instance };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuOptions).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, RogueMethodArgument.Identity);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
