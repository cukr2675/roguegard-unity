using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenuScreen : RogueMenuScreen
    {
        private readonly ClickItemHandler<FileInfo, MMgr, MArg> selectCallback;
        private readonly MainMenuViewTemplate<MMgr, MArg> view;

        public override bool IsIncremental => true;

        public SelectFileCommandMenuScreen(ClickItemHandler<FileInfo, MMgr, MArg> selectCallback)
        {
            this.selectCallback = selectCallback;

            view = new()
            {
                PrimaryCommandSubviewName = StandardSubviewTable.SecondaryCommandName,
            };
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            var text = RogueFile.GetName(((FileInfo)arg.Arg.Other).FullName) + "をロードしますか？";
            view.Title = text;

            view.ShowTemplate(manager, arg)
                ?
                .Option(":Load", (manager, arg) =>
                {
                    selectCallback((FileInfo)arg.Arg.Other, manager, arg);
                })

                .Option(":Rename", new RenameDialog())

                .Option(":Export", (manager, arg) =>
                {
                    RogueFile.Export(((FileInfo)arg.Arg.Other).FullName);
                    manager.PopMenuScreen();
                })

                .Option("<#f00>:Delete", new ChoicesMenuScreen(":DeleteMsg").Option("<#f00>:Delete", DeleteYes).Back())

                .Back()

                .Build();
        }

        private static void DeleteYes(MMgr manager, MArg arg)
        {
            var fileInfo = (FileInfo)arg.Arg.Other;
            fileInfo.Delete();
            manager.PopMenuScreen(2);
        }

        public override void CloseScreenView(MMgr manager, bool back)
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

            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                BackAnchorSubviewName = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var fileInfo = (FileInfo)arg.Arg.Other;
                newName = Path.GetFileNameWithoutExtension(fileInfo.Name);

                view.ShowTemplate("", manager, arg)
                    ?
                    .Tail(InputFieldViewWidget.CreateOption<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            return newName;
                        },
                        (manager, arg, value) =>
                        {
                            var invalidCharIndex = value.IndexOfAny(Path.GetInvalidFileNameChars());
                            if (invalidCharIndex >= 0) return newName;
                            else return newName = value;
                        })
                    )

                    .VarOnce(out var overwriteDialog, new ChoicesMenuScreen(":RenameOverride").Option(":Yes", Overwrite).Back())
                    .Tail(new object[]
                    {
                        SelectOption.Create<MMgr, MArg>(":Rename", (manager, arg) =>
                        {
                            if (string.IsNullOrWhiteSpace(newName))return;

                            var fileInfo = (FileInfo)arg.Arg.Other;
                            var newPath = Path.Combine(fileInfo.DirectoryName, $"{newName}{Path.GetExtension(fileInfo.Name)}");
                            if (newName != Path.GetFileNameWithoutExtension(fileInfo.Name) && File.Exists(newPath))
                            {
                                manager.PopMenuScreen(2);
                                manager.PushMenuScreen(overwriteDialog, other: new Paths() { path = fileInfo.FullName, newPath = newPath });
                            }
                            else
                            {
                                fileInfo.MoveTo(newPath);
                                manager.PopMenuScreen(2);
                            }
                        }),
                        BackSelectOption.Instance
                    })

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.HideTemplate(manager, back);
            }

            private static void Overwrite(MMgr manager, MArg arg)
            {
                manager.PopMenuScreen();

                var paths = (Paths)arg.Arg.Other;
                File.Delete(paths.newPath);
                File.Move(paths.path, paths.newPath);
                manager.Reopen();
            }
        }
    }
}
