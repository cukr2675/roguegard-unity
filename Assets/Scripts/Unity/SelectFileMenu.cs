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

        private readonly List<object> objs = new List<object>();

        private static readonly ItemController itemController = new ItemController();
        private static readonly ImportChoice importChoice = new ImportChoice();

        public delegate void SelectCallback(IModelsMenuRoot root, string path);
        public delegate void AddCallback(IModelsMenuRoot root);

        public SelectFileMenu(ScrollModelsMenuView scrollMenuView)
        {
            this.scrollMenuView = scrollMenuView;
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
                scrollMenuView.ShowExitButton(ExitModelsMenuChoice.Instance);
                if (itemController.type == Type.Read) { scrollMenuView.ShowSortButton(importChoice); }
                scrollMenuView.SetPosition(1f);
            });
        }

        private class ItemController : IModelsMenuItemController
        {
            public Type type;
            public SelectCallback selectCallback;
            public AddCallback addCallback;

            private CommandMenu nextMenu;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "+ New File";
                return (string)model;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (nextMenu == null)
                {
                    nextMenu = new CommandMenu() { parent = this };
                }

                if (model == null)
                {
                    addCallback(root);
                }
                else
                {
                    if (type == Type.Write)
                    {
                        selectCallback(root, (string)model);
                    }
                    else
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.OpenMenuAsDialog(nextMenu, null, null, new(other: model), RogueMethodArgument.Identity);
                    }
                }
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
                    return "ロード";
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
                    return "名称変更";
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
                    return "エクスポート";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    RogueFile.Export((string)arg.Other);
                }
            }

            private class Delete : IModelsMenuChoice
            {
                private static readonly DeleteDialog nextMenu = new DeleteDialog();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "<#f00>削除";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.AddInt(DeviceKw.StartTalk, 0);
                    root.AddObject(DeviceKw.AppendText, "削除すると戻せませんが本当に削除しますか？");
                    root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                    root.OpenMenuAsDialog(nextMenu, null, null, arg, arg);
                }
            }

            private class DeleteDialog : IModelsMenu
            {
                private static readonly object[] models = new object[]
                {
                    new ActionModelsMenuChoice("<#f00>削除する", Yes),
                    ExitModelsMenuChoice.Instance
                };

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
                }

                private static void Yes(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    RogueFile.Delete((string)arg.Other);
                    root.Back();
                }
            }
        }

        private class ImportChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "インポート";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                RogueFile.Import(StandardRogueDeviceSave.RootDirectory, x =>
                {
                    root.Reopen(self, user, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
                    root.Back();
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
