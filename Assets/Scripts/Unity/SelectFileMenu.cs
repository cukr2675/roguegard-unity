using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    public class SelectFileMenu : IModelsMenu
    {
        private readonly ScrollModelsMenuView scrollMenuView;
        private readonly IModelsMenuView leftAnchorMenuView;

        private readonly List<object> objs = new List<object>();
        private readonly List<object> leftAnchorObjs = new List<object>();

        private static readonly ItemController itemController = new ItemController();
        private static readonly ImportChoice importChoice = new ImportChoice();

        public delegate void SelectCallback(IModelsMenuRoot root, string path);
        public delegate void AddCallback(IModelsMenuRoot root);

        public SelectFileMenu(ScrollModelsMenuView scrollMenuView, IModelsMenuView leftAnchorMenuView)
        {
            this.scrollMenuView = scrollMenuView;
            this.leftAnchorMenuView = leftAnchorMenuView;
        }

        public void Open(Type type, SelectCallback selectCallback, AddCallback addCallback = null)
        {
            SetCallback(type, selectCallback, addCallback);
            OpenMenu(null, null, null, RogueMethodArgument.Identity);
        }

        public void SetCallback(Type type, SelectCallback selectCallback, AddCallback addCallback = null)
        {
            itemController.type = type;
            itemController.selectCallback = selectCallback;
            itemController.addCallback = addCallback;
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!Directory.Exists(StandardRogueDeviceSave.RootDirectory))
            {
                Directory.CreateDirectory(StandardRogueDeviceSave.RootDirectory);
            }

            var filesArg = arg;
            StandardRogueDeviceSave.GetFiles(files =>
            {
                objs.Clear();
                if (itemController.addCallback != null) { objs.Add(null); }
                objs.AddRange(files);
                scrollMenuView.OpenView(itemController, objs, root, self, user, filesArg);
                scrollMenuView.SetPosition(0f);

                leftAnchorObjs.Clear();
                if (itemController.type == Type.Read) { leftAnchorObjs.Add(importChoice); }
                leftAnchorObjs.Add(ExitModelsMenuChoice.Instance);
                leftAnchorMenuView.OpenView(ChoicesModelsMenuItemController.Instance, leftAnchorObjs, root, self, user, filesArg);
            });
        }

        private class ItemController : IModelsMenuItemController
        {
            public Type type;
            public SelectCallback selectCallback;
            public AddCallback addCallback;

            private CommandMenu nextMenu;
            private DialogModelsMenuChoice overwriteDialog;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return ":+ New File";
                
                var file = (RogueFile)model;
                return file.Path.Substring(file.Path.LastIndexOf('/') + 1);
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (nextMenu == null)
                {
                    nextMenu = new CommandMenu() { parent = this };
                    overwriteDialog = new DialogModelsMenuChoice((":Overwrite", Yes)).AppendExit();
                }

                if (model == null)
                {
                    addCallback(root);
                }
                else
                {
                    var file = (RogueFile)model;
                    if (type == Type.Write)
                    {
                        root.AddInt(DeviceKw.StartTalk, 0);
                        root.AddObject(DeviceKw.AppendText, ":OverwriteMsg::1");
                        root.AddObject(DeviceKw.AppendText, RogueFile.GetName(file.Path));
                        root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                        root.OpenMenuAsDialog(overwriteDialog, null, null, new(other: file.Path), RogueMethodArgument.Identity);
                    }
                    else
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.OpenMenuAsDialog(nextMenu, null, null, new(other: file.Path), RogueMethodArgument.Identity);
                    }
                }
            }

            private void Yes(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // セーブデータを上書き
                selectCallback(root, (string)arg.Other);
            }
        }

        private class CommandMenu : IModelsMenu
        {
            public ItemController parent;

            private object[] models;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (models == null)
                {
                    models = new object[]
                    {
                        new Load() { parent = parent },
                        new Rename(),
                        new Delete(),
                        new Export(),
                        ExitModelsMenuChoice.Instance
                    };
                }
                
                var menu = root.Get(DeviceKw.MenuCommand);
                menu.OpenView(ChoicesModelsMenuItemController.Instance, models, root, null, null, new(other: arg.Other));
            }

            private class Load : IModelsMenuChoice
            {
                public ItemController parent;

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ":Load";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    parent.selectCallback(root, (string)arg.Other);
                }
            }

            private class Rename : IModelsMenuChoice
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ":Rename";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var path = (string)arg.Other;
                    var newPath = Path.Combine(Path.GetDirectoryName(path), "NewPath.gard");
                    RogueFile.Move(path, newPath, x =>
                    {
                        root.AddObject(DeviceKw.EnqueueSE, x ? DeviceKw.Submit : DeviceKw.Cancel);
                    });
                    root.Back();
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
                    RogueFile.Export((string)arg.Other);
                }
            }

            private class Delete : IModelsMenuChoice
            {
                private static readonly DialogModelsMenuChoice nextMenu = new DialogModelsMenuChoice(("<#f00>:Delete", Yes)).AppendExit();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "<#f00>:Delete";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.AddInt(DeviceKw.StartTalk, 0);
                    root.AddObject(DeviceKw.AppendText, ":DeleteMsg");
                    root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                    root.OpenMenuAsDialog(nextMenu, null, null, arg, arg);
                }
            }

            private static void Yes(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                RogueFile.Delete((string)arg.Other);
                root.Back();
            }
        }

        private class ImportChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ":Import";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                RogueFile.Import(StandardRogueDeviceSave.RootDirectory, x =>
                {
                    root.Reopen(self, user, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                });
            }
        }

        public enum Type
        {
            Read,
            Write
        }
    }
}
