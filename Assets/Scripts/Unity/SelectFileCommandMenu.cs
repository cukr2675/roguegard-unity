using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenu : RogueMenuScreen
    {
        private readonly SelectFileMenu.SelectCallback selectCallback;
        private readonly DialogListMenuSelectOption overwriteDialog;
        private readonly DialogListMenuSelectOption deleteMenu = new DialogListMenuSelectOption(("<#f00>:Delete", DeleteYes)).AppendExit();

        private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public SelectFileCommandMenu(SelectFileMenu.SelectCallback selectCallback)
        {
            this.selectCallback = selectCallback;
            overwriteDialog = new DialogListMenuSelectOption("", ":RenameOverride", (":Yes", Overwrite)).AppendExit();
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            var text = RogueFile.GetName((string)arg.Arg.Other) + "‚ðƒ[ƒh‚µ‚Ü‚·‚©H";
            view.Title = text;

            view.Show(manager, arg)
                ?.Option(":Load", (manager, arg) =>
                {
                    selectCallback(manager, (string)arg.Arg.Other);
                })
                .Option(":Rename", (manager, arg) =>
                {
                    manager.HandleClickBack();
                    SelectFileMenu.ShowSaving(manager);

                    var path = (string)arg.Arg.Other;
                    var newPath = Path.Combine(Path.GetDirectoryName(path), "NewPath.gard");
                    if (RogueFile.Exists(newPath))
                    {
                        manager.HandleClickBack();
                        manager.PushMenuScreen(overwriteDialog.menuScreen, arg.Self, arg.User, other: new Paths() { path = path, newPath = newPath });
                    }
                    else
                    {
                        RogueFile.Move(path, newPath);
                        manager.Reopen();
                    }
                })
                .Option(":Export", (manager, arg) =>
                {
                    manager.HandleClickBack();
                    SelectFileMenu.ShowLoading(manager);

                    RogueFile.Export((string)arg.Arg.Other);
                    manager.Reopen();
                })
                .Option(":Delete", (manager, arg) =>
                {
                    manager.AddInt(DeviceKw.StartTalk, 0);
                    manager.AddObject(DeviceKw.AppendText, ":DeleteMsg");
                    manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                    manager.PushMenuScreen(deleteMenu.menuScreen, arg);
                })
                .Build();
        }

        private void Overwrite(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.HandleClickBack();
            SelectFileMenu.ShowSaving(manager);

            var paths = (Paths)arg.Arg.Other;
            RogueFile.Delete(paths.newPath);
            RogueFile.Move(paths.path, paths.newPath);
            manager.Reopen();
        }

        private static void DeleteYes(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.HandleClickBack();
            SelectFileMenu.ShowDeleting(manager);

            RogueFile.Delete((string)arg.Arg.Other);
            manager.Reopen();
        }

        private class Paths
        {
            public string path, newPath;
        }
    }
}
