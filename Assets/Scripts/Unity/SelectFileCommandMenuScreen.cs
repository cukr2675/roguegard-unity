using Lysionium;
using Roguegard.Device;
using System.IO;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenuScreen : RogueMenuScreen
    {
        private readonly ClickItemHandler<FileInfo, MMgr, MArg> selectCallback;
        private readonly MainMenuViewData<MMgr, MArg> view;

        public override bool IsIncremental => true;

        public SelectFileCommandMenuScreen(ClickItemHandler<FileInfo, MMgr, MArg> selectCallback)
        {
            this.selectCallback = selectCallback;

            view = new()
            {
                PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
            };
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            var text = RogueFile.GetName(((FileInfo)arg.Arg.Other).FullName) + "をロードしますか？";
            view.Title = text;

            view.Show(manager, arg)
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
            view.Hide(manager, back);
        }

        private class Paths
        {
            public string path, newPath;
        }

        private class RenameDialog : RogueMenuScreen
        {
            private string newName;

            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                var fileInfo = (FileInfo)arg.Arg.Other;
                newName = Path.GetFileNameWithoutExtension(fileInfo.Name);

                view.Show("", manager, arg)
                    ?
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            return newName;
                        },
                        (manager, arg, value) =>
                        {
                            var invalidCharIndex = value.IndexOfAny(Path.GetInvalidFileNameChars());
                            if (invalidCharIndex >= 0) return newName;
                            else return newName = value;
                        }))

                    .VarOnce(out var overwriteDialog, new ChoicesMenuScreen(":RenameOverride").Option(":Yes", Overwrite).Back())
                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr, MArg>(":Rename", (manager, arg) =>
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
                        })),
                        ("1*", BackSelectOption<MMgr, MArg>.Instance)))

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
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
