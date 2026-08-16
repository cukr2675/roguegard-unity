using Lysionium;
using Roguegard.Device;
using System.IO;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenuScreen : RogueListuiScreen
    {
        private readonly MainMenuViewData<MMgr> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public SelectFileCommandMenuScreen(System.Action<FileInfo, MMgr> selectCallback)
        {
            OnOpenScreen += (manager) =>
            {
                var text = RogueFile.GetName(((FileInfo)Arg.Arg.Other).FullName) + "をロードしますか？";
                view.Title = text;

                view.Show(manager)
                ?
                .Option(":Load", (manager) =>
                {
                    selectCallback((FileInfo)Arg.Arg.Other, manager);
                })

                .Option(":Rename", new RenameDialog(), () => Arg)

                .Option(":Export", (manager) =>
                {
                    RogueFile.Export(((FileInfo)Arg.Arg.Other).FullName);
                    manager.PopScreen();
                })

                .Option("<#f00>:Delete", new ChoicesScreen(":DeleteMsg").Option("<#f00>:Delete", DeleteYes).Back(), () => Arg)

                .Back()

                .Build();
            };

            OnCloseScreenView += (manager, back) =>
            {
                view.Hide(manager, back);
            };
        }

        private void DeleteYes(MMgr manager)
        {
            var fileInfo = (FileInfo)Arg.Arg.Other;
            fileInfo.Delete();
            manager.PopScreen(2);
        }

        private class Paths
        {
            public string path, newPath;
        }

        private class RenameDialog : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public RenameDialog()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show("", manager)
                    ?
                    .VarOnce(out string newName)
                    .Init(() =>
                    {
                        var fileInfo = (FileInfo)Arg.Arg.Other;
                        newName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    })

                    .Tail.Append(InputFieldWidgetOption.Create<MMgr>(
                        _ =>
                        {
                            return newName;
                        },
                        value =>
                        {
                            var invalidCharIndex = value.IndexOfAny(Path.GetInvalidFileNameChars());
                            if (invalidCharIndex >= 0) return newName;
                            else return newName = value;
                        }))

                    .VarOnce(out var overwriteDialog, new ChoicesScreen(":RenameOverride").Option(":Yes", Overwrite).Back())
                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr>(":Rename", (manager) =>
                        {
                            if (string.IsNullOrWhiteSpace(newName)) return;

                            var fileInfo = (FileInfo)Arg.Arg.Other;
                            var newPath = Path.Combine(fileInfo.DirectoryName, $"{newName}{Path.GetExtension(fileInfo.Name)}");
                            if (newName != Path.GetFileNameWithoutExtension(fileInfo.Name) && File.Exists(newPath))
                            {
                                manager.PopScreen(2);
                                manager.PushScreen(overwriteDialog, other: new Paths() { path = fileInfo.FullName, newPath = newPath });
                            }
                            else
                            {
                                fileInfo.MoveTo(newPath);
                                manager.PopScreen(2);
                            }
                        })),
                        ("1*", BackSelectOption<MMgr>.Instance)))

                    .Build();
                };

                OnCloseScreenView += (manager, back) =>
                {
                    view.Hide(manager, back);
                };
            }

            private void Overwrite(MMgr manager)
            {
                manager.PopScreen();

                var paths = (Paths)Arg.Arg.Other;
                File.Delete(paths.newPath);
                File.Move(paths.path, paths.newPath);
                manager.Reopen();
            }
        }
    }
}
