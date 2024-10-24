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
        private readonly HandleClickElement<FileInfo, RogueMenuManager, ReadOnlyMenuArg> selectCallback;
        private readonly RenameDialog renameDialog = new();
        private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view;

        public override bool IsIncremental => true;

        public SelectFileCommandMenu(HandleClickElement<FileInfo, RogueMenuManager, ReadOnlyMenuArg> selectCallback)
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

            view.Show(manager, arg)
                ?
                .Option(":Load", (manager, arg) =>
                {
                    selectCallback((FileInfo)arg.Arg.Other, manager, arg);
                })

                .Option(":Rename", renameDialog)

                .Option(":Export", (manager, arg) =>
                {
                    manager.HandleClickBack();

                    RogueFile.Export((string)arg.Arg.Other);
                    manager.Reopen();
                })

                .Option(":Delete", new ChoicesMenuScreen(":DeleteMsg").Option("<#f00>:Delete", DeleteYes).Exit())

                .Exit()

                .Build();
        }

        private static void DeleteYes(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.HandleClickBack();

            var fileInfo = (FileInfo)arg.Arg.Other;
            fileInfo.Delete();
            manager.Reopen();
        }

        public override void CloseScreen(RogueMenuManager manager, bool back)
        {
            view.HideSubViews(manager, back);
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

                view.Show("", manager, arg)
                    ?
                    .Append(InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
                        (manager, arg) =>
                        {
                            var fileInfo = (FileInfo)arg.Arg.Other;
                            return fileInfo.Name;
                        },
                        (manager, arg, value) =>
                        {
                            newName = value;
                        })
                    )

                    .VariableOnce(out var overwriteDialog, new ChoicesMenuScreen(":RenameOverride").Option(":Yes", Overwrite).Exit())
                    .Append(new object[]
                    {
                        ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Rename", (manager, arg) =>
                        {
                            var fileInfo = (FileInfo)arg.Arg.Other;
                            var newPath = Path.Combine(fileInfo.DirectoryName, newName);
                            if (File.Exists(newPath))
                            {
                                manager.HandleClickBack();
                                manager.PushMenuScreen(overwriteDialog, other: new Paths() { path = fileInfo.FullName, newPath = newPath });
                            }
                            else
                            {
                                manager.HandleClickBack();
                                fileInfo.MoveTo(newPath);
                                manager.Reopen();
                            }
                        }),
                        ExitListMenuSelectOption.Instance
                    })

                    .Build();
            }

            public override void CloseScreen(RogueMenuManager manager, bool back)
            {
                view.HideSubViews(manager, back);
            }

            private static void Overwrite(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                manager.HandleClickBack();

                var paths = (Paths)arg.Arg.Other;
                File.Delete(paths.newPath);
                File.Move(paths.path, paths.newPath);
                manager.Reopen();
            }
        }
    }
}
