using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class SelectFileMenu : IModelsMenu
    {
        private readonly ItemController itemController;

        private readonly List<object> objs = new();
        private readonly List<object> leftAnchorObjs = new();

        private static readonly ImportChoice importChoice = new();
        private static readonly LoadingModelsMenu savingMenu = new LoadingModelsMenu("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly LoadingModelsMenu loadingMenu = new LoadingModelsMenu("ロード中…", "キャンセル", LoadingCancel);
        private static readonly LoadingModelsMenu deletingMenu = new LoadingModelsMenu("削除中…", "キャンセル", LoadingCancel);
        private static readonly DialogModelsMenuChoice errorMsgDialog = new DialogModelsMenuChoice(("OK", ErrorMsgOK));

        public delegate void SelectCallback(IModelsMenuRoot root, string path);
        public delegate void AddCallback(IModelsMenuRoot root);

        public SelectFileMenu(Type type, SelectCallback selectCallback, AddCallback addCallback = null)
        {
            itemController = new ItemController();
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

                var scroll = root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(itemController, objs, root, self, user, filesArg);
                scroll.SetPosition(0f);

                var caption = root.Get(DeviceKw.MenuCaption);
                caption.OpenView(
                    ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, null, null,
                    new(other: itemController.type == Type.Read ? ":Load" : ":Save"));

                leftAnchorObjs.Clear();
                if (itemController.type == Type.Read) { leftAnchorObjs.Add(importChoice); }
                leftAnchorObjs.Add(ExitModelsMenuChoice.Instance);
                var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
                leftAnchor.OpenView(ChoicesModelsMenuItemController.Instance, leftAnchorObjs, root, self, user, filesArg);
            });
        }

        public static void ShowSaving(IModelsMenuRoot root)
        {
            root.OpenMenuAsDialog(savingMenu, null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
        }

        public static void ShowLoading(IModelsMenuRoot root)
        {
            root.OpenMenuAsDialog(loadingMenu, null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
        }

        public static void ShowDeleting(IModelsMenuRoot root)
        {
            root.OpenMenuAsDialog(deletingMenu, null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
        }

        private static void LoadingCancel(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
        }

        public static void ReopenCallback(IModelsMenuRoot root, string errorMsg)
        {
            if (errorMsg != null)
            {
                root.Back();
                ShowErrorMsg(root, errorMsg);
                return;
            }

            root.Reopen(null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
        }

        public static void ShowErrorMsg(IModelsMenuRoot root, string errorMsg)
        {
            root.AddInt(DeviceKw.StartTalk, 0);
            root.AddObject(DeviceKw.AppendText, ":An error has occurred.");
            root.AddObject(DeviceKw.AppendText, " (");
            root.AddObject(DeviceKw.AppendText, errorMsg);
            root.AddObject(DeviceKw.AppendText, ")");
            root.AddInt(DeviceKw.WaitEndOfTalk, 0);
            root.OpenMenuAsDialog(errorMsgDialog, null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
        }

        private static void ErrorMsgOK(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            root.Back();
        }

        private class ItemController : IModelsMenuItemController
        {
            public Type type;
            public SelectCallback selectCallback;
            public AddCallback addCallback;

            private SelectFileCommandMenu nextMenu;
            private DialogModelsMenuChoice overwriteDialog;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return ":+ New File";
                
                var file = (RogueFile)model;
                return file.Path.Substring(file.Path.LastIndexOf('/') + 1);
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null)
                {
                    addCallback(root);
                }
                else
                {
                    var file = (RogueFile)model;
                    if (type == Type.Write)
                    {
                        if (overwriteDialog == null) { overwriteDialog = new DialogModelsMenuChoice((":Overwrite", Yes)).AppendExit(); }

                        root.AddInt(DeviceKw.StartTalk, 0);
                        root.AddObject(DeviceKw.AppendText, ":OverwriteMsg::1");
                        root.AddObject(DeviceKw.AppendText, RogueFile.GetName(file.Path));
                        root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                        root.OpenMenuAsDialog(overwriteDialog, null, null, new(other: file.Path), RogueMethodArgument.Identity);
                    }
                    else
                    {
                        if (nextMenu == null) { nextMenu = new SelectFileCommandMenu(selectCallback); }

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

        private class ImportChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ":Import";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                ShowSaving(root);
                RogueFile.Import(StandardRogueDeviceSave.RootDirectory, errorMsg =>
                {
                    if (errorMsg != null)
                    {
                        ShowErrorMsg(root, errorMsg);
                        return;
                    }

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
