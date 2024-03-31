using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileCommandMenu : IModelsMenu
    {
        private readonly object[] models;

        public SelectFileCommandMenu(SelectFileMenu.SelectCallback selectCallback)
        {
            models = new object[]
            {
                new Load() { selectCallback = selectCallback },
                new Rename(),
                new Delete(),
                new Export(),
                ExitModelsMenuChoice.Instance
            };
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var command = root.Get(DeviceKw.MenuCommand);
            command.OpenView(ChoicesModelsMenuItemController.Instance, models, root, null, null, new(other: arg.Other));

            var caption = root.Get(DeviceKw.MenuCaption);
            var text = RogueFile.GetName((string)arg.Other) + "�����[�h���܂����H";
            caption.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, null, null, new(other: text));
        }

        private class Load : IModelsMenuChoice
        {
            public SelectFileMenu.SelectCallback selectCallback;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Load";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                selectCallback(root, (string)arg.Other);
            }
        }

        private class Rename : IModelsMenuChoice
        {
            private readonly DialogModelsMenuChoice overwriteDialog;

            public Rename()
            {
                overwriteDialog = new DialogModelsMenuChoice("", ":RenameOverride", (":Yes", Overwrite)).AppendExit();
            }

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Rename";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                SelectFileMenu.ShowSaving(root);

                var path = (string)arg.Other;
                var newPath = Path.Combine(Path.GetDirectoryName(path), "NewPath.gard");
                RogueFile.Exists(newPath, (exists, errorMsg) =>
                {
                    if (errorMsg != null)
                    {
                        root.Back();
                        SelectFileMenu.ShowErrorMsg(root, errorMsg);
                        return;
                    }

                    if (exists)
                    {
                        var newArg = new RogueMethodArgument(other: new Paths() { path = path, newPath = newPath });
                        root.Back();
                        root.OpenMenuAsDialog(overwriteDialog, self, user, newArg);
                    }
                    else
                    {
                        RogueFile.Move(path, newPath, errorMsg => SelectFileMenu.ReopenCallback(root, errorMsg));
                    }
                });
            }

            private void Overwrite(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                SelectFileMenu.ShowSaving(root);

                var paths = (Paths)arg.Other;
                RogueFile.Move(paths.path, paths.newPath, errorMsg => SelectFileMenu.ReopenCallback(root, errorMsg), true);
            }

            private class Paths
            {
                public string path, newPath;
            }
        }

        private class Export : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ":Export";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                SelectFileMenu.ShowLoading(root);

                RogueFile.Export((string)arg.Other, errorMsg => SelectFileMenu.ReopenCallback(root, errorMsg));
            }
        }

        private class Delete : IModelsMenuChoice
        {
            private static readonly DialogModelsMenuChoice nextMenu = new DialogModelsMenuChoice(("<#f00>:Delete", Yes)).AppendExit();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "<#f00>:Delete";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddInt(DeviceKw.StartTalk, 0);
                root.AddObject(DeviceKw.AppendText, ":DeleteMsg");
                root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                root.OpenMenuAsDialog(nextMenu, null, null, arg);
            }

            private static void Yes(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                SelectFileMenu.ShowDeleting(root);

                RogueFile.Delete((string)arg.Other, errorMsg => SelectFileMenu.ReopenCallback(root, errorMsg));
            }
        }
    }
}