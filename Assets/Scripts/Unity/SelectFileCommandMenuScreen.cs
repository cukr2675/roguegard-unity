using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ListingMF;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenuScreen : RogueMenuScreen
    {
        private readonly HandleClickElement<FileInfo, RogueMenuManager, ReadOnlyMenuArg> selectCallback;
        private readonly RenameDialog renameDialog = new();
        private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view;

        public override bool IsIncremental => true;

        public SelectFileCommandMenuScreen(HandleClickElement<FileInfo, RogueMenuManager, ReadOnlyMenuArg> selectCallback)
        {
            this.selectCallback = selectCallback;

            view = new()
            {
                PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
            };
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            var text = RogueFile.GetName(((FileInfo)arg.Arg.Other).FullName) + "‚ðƒ[ƒh‚µ‚Ü‚·‚©H";
            view.Title = text;

            view.ShowTemplate(manager, arg)
                ?
                .Option(":Load", (manager, arg) =>
                {
                    selectCallback((FileInfo)arg.Arg.Other, manager, arg);
                })

                .Option(":Rename", renameDialog)

                .Option(":Export", (manager, arg) =>
                {
                    manager.Back();

                    RogueFile.Export(((FileInfo)arg.Arg.Other).FullName);
                    manager.Reopen();
                })

                .Option(":Delete", new ChoicesMenuScreen(":DeleteMsg").Option("<#f00>:Delete", DeleteYes).Back())

                .Back()

                .Build();
        }

        private static void DeleteYes(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.Back();

            var fileInfo = (FileInfo)arg.Arg.Other;
            fileInfo.Delete();
            manager.Reopen();
        }

        public override void CloseScreen(RogueMenuManager manager, bool back)
        {
            view.HideTemplate(manager, back);
        }

        private class Paths
        {
            public string path, newPath;
        }

        private class RenameDialog : RogueMenuScreen
        {
            private string newName;

            private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var fileInfo = (FileInfo)arg.Arg.Other;
                newName = fileInfo.Name;

                view.ShowTemplate("", manager, arg)
                    ?
                    .Append(InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
                        (manager, arg) =>
                        {
                            var fileInfo = (FileInfo)arg.Arg.Other;
                            return fileInfo.Name;
                        },
                        (manager, arg, value) =>
                        {
                            return newName = value;
                        })
                    )

                    .VariableOnce(out var overwriteDialog, new ChoicesMenuScreen(":RenameOverride").Option(":Yes", Overwrite).Back())
                    .Append(new object[]
                    {
                        SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Rename", (manager, arg) =>
                        {
                            var fileInfo = (FileInfo)arg.Arg.Other;
                            var newPath = Path.Combine(fileInfo.DirectoryName, newName);
                            if (File.Exists(newPath))
                            {
                                manager.Back();
                                manager.PushMenuScreen(overwriteDialog, other: new Paths() { path = fileInfo.FullName, newPath = newPath });
                            }
                            else
                            {
                                manager.Back();
                                fileInfo.MoveTo(newPath);
                                manager.Reopen();
                            }
                        }),
                        BackSelectOption.Instance
                    })

                    .Build();
            }

            public override void CloseScreen(RogueMenuManager manager, bool back)
            {
                view.HideTemplate(manager, back);
            }

            private static void Overwrite(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                manager.Back();

                var paths = (Paths)arg.Arg.Other;
                File.Delete(paths.newPath);
                File.Move(paths.path, paths.newPath);
                manager.Reopen();
            }
        }
    }
}
