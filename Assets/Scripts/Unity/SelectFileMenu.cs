using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class SelectFileMenu : IListMenu
    {
        private readonly Presenter presenter;

        private readonly List<object> objs = new();
        private readonly List<object> leftAnchorObjs = new();

        private static readonly ImportSelectOption importSelectOption = new();
        private static readonly LoadingListMenu savingMenu = new LoadingListMenu("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly LoadingListMenu loadingMenu = new LoadingListMenu("ロード中…", "キャンセル", LoadingCancel);
        private static readonly LoadingListMenu deletingMenu = new LoadingListMenu("削除中…", "キャンセル", LoadingCancel);
        private static readonly DialogListMenuSelectOption errorMsgDialog = new DialogListMenuSelectOption(("OK", ErrorMsgOK));

        public delegate void SelectCallback(IListMenuManager manager, string path);
        public delegate void AddCallback(IListMenuManager manager);

        public SelectFileMenu(Type type, SelectCallback selectCallback, AddCallback addCallback = null)
        {
            presenter = new Presenter();
            presenter.type = type;
            presenter.selectCallback = selectCallback;
            presenter.addCallback = addCallback;
        }

        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            RogueFile.InitializeDirectory(StandardRogueDeviceSave.RootDirectory);

            var filesArg = arg;
            StandardRogueDeviceSave.GetFiles(files =>
            {
                objs.Clear();
                if (presenter.addCallback != null) { objs.Add(null); }
                objs.AddRange(files);

                var scroll = manager.GetView(DeviceKw.MenuScroll);
                scroll.OpenView(presenter, objs, manager, self, user, filesArg);
                scroll.SetPosition(0f);

                var caption = manager.GetView(DeviceKw.MenuCaption);
                caption.OpenView(
                    SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, null, null,
                    new(other: presenter.type == Type.Read ? ":Load" : ":Save"));

                leftAnchorObjs.Clear();
                if (presenter.type == Type.Read) { leftAnchorObjs.Add(importSelectOption); }
                leftAnchorObjs.Add(ExitListMenuSelectOption.Instance);
                var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
                leftAnchor.OpenView(SelectOptionPresenter.Instance, leftAnchorObjs, manager, self, user, filesArg);
            });
        }

        public static void ShowSaving(IListMenuManager manager)
        {
            manager.OpenMenuAsDialog(savingMenu, null, null, RogueMethodArgument.Identity);
        }

        public static void ShowLoading(IListMenuManager manager)
        {
            manager.OpenMenuAsDialog(loadingMenu, null, null, RogueMethodArgument.Identity);
        }

        public static void ShowDeleting(IListMenuManager manager)
        {
            manager.OpenMenuAsDialog(deletingMenu, null, null, RogueMethodArgument.Identity);
        }

        private static void LoadingCancel(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
        }

        public static void ReopenCallback(IListMenuManager manager, string errorMsg)
        {
            if (errorMsg != null)
            {
                manager.Back();
                ShowErrorMsg(manager, errorMsg);
                return;
            }

            manager.Reopen();
        }

        public static void ShowErrorMsg(IListMenuManager manager, string errorMsg)
        {
            manager.AddInt(DeviceKw.StartTalk, 0);
            manager.AddObject(DeviceKw.AppendText, ":An error has occurred.");
            manager.AddObject(DeviceKw.AppendText, " (");
            manager.AddObject(DeviceKw.AppendText, errorMsg);
            manager.AddObject(DeviceKw.AppendText, ")");
            manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
            manager.OpenMenuAsDialog(errorMsgDialog, null, null, RogueMethodArgument.Identity);
        }

        private static void ErrorMsgOK(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            manager.Back();
        }

        private class Presenter : IElementPresenter
        {
            public Type type;
            public SelectCallback selectCallback;
            public AddCallback addCallback;

            private SelectFileCommandMenu nextMenu;
            private DialogListMenuSelectOption overwriteDialog;

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null) return ":+ New File";
                
                var file = (RogueFile)element;
                return file.Path.Substring(file.Path.LastIndexOf('/') + 1);
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null)
                {
                    addCallback(manager);
                }
                else
                {
                    var file = (RogueFile)element;
                    if (type == Type.Write)
                    {
                        if (overwriteDialog == null) { overwriteDialog = new DialogListMenuSelectOption((":Overwrite", Yes)).AppendExit(); }

                        manager.AddInt(DeviceKw.StartTalk, 0);
                        manager.AddObject(DeviceKw.AppendText, ":OverwriteMsg::1");
                        manager.AddObject(DeviceKw.AppendText, RogueFile.GetName(file.Path));
                        manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                        manager.OpenMenuAsDialog(overwriteDialog, null, null, new(other: file.Path));
                    }
                    else
                    {
                        if (nextMenu == null) { nextMenu = new SelectFileCommandMenu(selectCallback); }

                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        manager.OpenMenuAsDialog(nextMenu, null, null, new(other: file.Path));
                    }
                }
            }

            private void Yes(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // セーブデータを上書き
                selectCallback(manager, (string)arg.Other);
            }
        }

        private class ImportSelectOption : BaseListMenuSelectOption
        {
            public override string Name => ":Import";

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                ShowSaving(manager);
                RogueFile.Import(StandardRogueDeviceSave.RootDirectory, errorMsg =>
                {
                    if (errorMsg != null)
                    {
                        ShowErrorMsg(manager, errorMsg);
                        return;
                    }

                    manager.Reopen();
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
