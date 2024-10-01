using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenu : IListMenu
    {
        private readonly object[] elms;

        public SelectFileCommandMenu(SelectFileMenu.SelectCallback selectCallback)
        {
            elms = new object[]
            {
                new Load() { selectCallback = selectCallback },
                new Rename(),
                new Delete(),
                new Export(),
                ExitListMenuSelectOption.Instance
            };
        }

        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var command = manager.GetView(DeviceKw.MenuCommand);
            command.OpenView(SelectOptionPresenter.Instance, elms, manager, null, null, new(other: arg.Other));

            var caption = manager.GetView(DeviceKw.MenuCaption);
            var text = RogueFile.GetName((string)arg.Other) + "ÇÉçÅ[ÉhÇµÇ‹Ç∑Ç©ÅH";
            caption.OpenView(SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, null, null, new(other: text));
        }

        private class Load : BaseListMenuSelectOption
        {
            public override string Name => ":Load";

            public SelectFileMenu.SelectCallback selectCallback;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                selectCallback(manager, (string)arg.Other);
            }
        }

        private class Rename : BaseListMenuSelectOption
        {
            public override string Name => ":Rename";

            private readonly DialogListMenuSelectOption overwriteDialog;

            public Rename()
            {
                overwriteDialog = new DialogListMenuSelectOption("", ":RenameOverride", (":Yes", Overwrite)).AppendExit();
            }

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.Back();
                SelectFileMenu.ShowSaving(manager);

                var path = (string)arg.Other;
                var newPath = Path.Combine(Path.GetDirectoryName(path), "NewPath.gard");
                if (RogueFile.Exists(newPath))
                {
                    var newArg = new RogueMethodArgument(other: new Paths() { path = path, newPath = newPath });
                    manager.Back();
                    manager.OpenMenuAsDialog(overwriteDialog, self, user, newArg);
                }
                else
                {
                    RogueFile.Move(path, newPath);
                    manager.Reopen();
                }
            }

            private void Overwrite(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.Back();
                SelectFileMenu.ShowSaving(manager);

                var paths = (Paths)arg.Other;
                RogueFile.Delete(paths.newPath);
                RogueFile.Move(paths.path, paths.newPath);
                manager.Reopen();
            }

            private class Paths
            {
                public string path, newPath;
            }
        }

        private class Export : BaseListMenuSelectOption
        {
            public override string Name => ":Export";

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.Back();
                SelectFileMenu.ShowLoading(manager);

                RogueFile.Export((string)arg.Other);
                manager.Reopen();
            }
        }

        private class Delete : BaseListMenuSelectOption
        {
            public override string Name => "<#f00>:Delete";

            private static readonly DialogListMenuSelectOption nextMenu = new DialogListMenuSelectOption(("<#f00>:Delete", Yes)).AppendExit();

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.AddInt(DeviceKw.StartTalk, 0);
                manager.AddObject(DeviceKw.AppendText, ":DeleteMsg");
                manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                manager.OpenMenuAsDialog(nextMenu, null, null, arg);
            }

            private static void Yes(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.Back();
                SelectFileMenu.ShowDeleting(manager);

                RogueFile.Delete((string)arg.Other);
                manager.Reopen();
            }
        }
    }
}
