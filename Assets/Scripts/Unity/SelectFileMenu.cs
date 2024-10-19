using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class SelectFileMenu : RogueMenuScreen
    {
        private readonly Presenter presenter;

        private readonly List<RogueFile> objs = new();
        private readonly List<object> leftAnchorObjs = new();

        private static readonly LoadingListMenu savingMenu = new LoadingListMenu("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly LoadingListMenu loadingMenu = new LoadingListMenu("ロード中…", "キャンセル", LoadingCancel);
        private static readonly LoadingListMenu deletingMenu = new LoadingListMenu("削除中…", "キャンセル", LoadingCancel);
        private static readonly DialogListMenuSelectOption errorMsgDialog = new DialogListMenuSelectOption(("OK", ErrorMsgOK));

        private SelectFileCommandMenu nextMenu;
        private DialogListMenuSelectOption overwriteDialog;

        public delegate void SelectCallback(RogueMenuManager manager, string path);
        public delegate void AddCallback(RogueMenuManager manager);

        private readonly ScrollViewTemplate<RogueFile, RogueMenuManager, ReadOnlyMenuArg> readView = new()
        {
            Title = ":Load",
        };

        private readonly ScrollViewTemplate<RogueFile, RogueMenuManager, ReadOnlyMenuArg> writeView = new()
        {
            Title = ":Save",
            BackAnchorList = new IListMenuSelectOption[]
            {
                ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Import", (manager, arg) =>
                {
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
                }),
                ExitListMenuSelectOption.Instance,
            }
        };

        public SelectFileMenu(Type type, SelectCallback selectCallback, AddCallback addCallback = null)
        {
            presenter = new Presenter();
            presenter.type = type;
            presenter.selectCallback = selectCallback;
            presenter.addCallback = addCallback;
        }

        public override void OpenScreen(in RogueMenuManager inManager, in ReadOnlyMenuArg inArg)
        {
            var manager = inManager;
            var arg = inArg;
            RogueFile.InitializeDirectory(StandardRogueDeviceSave.RootDirectory);

            StandardRogueDeviceSave.GetFiles(files =>
            {
                objs.Clear();
                if (presenter.addCallback != null) { objs.Add(null); }
                objs.AddRange(files);

                if (presenter.type == Type.Read)
                {
                    readView.Show(objs, manager, arg)
                        ?.ElementNameGetter((file, manager, arg) =>
                        {
                            if (file == null) return ":+ New File";

                            return file.Path.Substring(file.Path.LastIndexOf('/') + 1);
                        })
                        .OnClickElement((file, manager, arg) =>
                        {
                            if (file == null)
                            {
                                presenter.addCallback(manager);
                            }
                            else
                            {
                                if (nextMenu == null) { nextMenu = new SelectFileCommandMenu(presenter.selectCallback); }

                                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                                manager.PushMenuScreen(nextMenu, other: file.Path);
                            }
                        })
                        .Build();
                }
                else // if (presenter.type == Type.Write)
                {
                    writeView.Show(objs, manager, arg)
                        ?.ElementNameGetter((file, manager, arg) =>
                        {
                            if (file == null) return ":+ New File";

                            return file.Path.Substring(file.Path.LastIndexOf('/') + 1);
                        })
                        .OnClickElement((file, manager, arg) =>
                        {
                            if (file == null)
                            {
                                presenter.addCallback(manager);
                            }
                            else
                            {
                                if (overwriteDialog == null) { overwriteDialog = new DialogListMenuSelectOption((":Overwrite", presenter.Yes)).AppendExit(); }

                                manager.AddInt(DeviceKw.StartTalk, 0);
                                manager.AddObject(DeviceKw.AppendText, ":OverwriteMsg::1");
                                manager.AddObject(DeviceKw.AppendText, RogueFile.GetName(file.Path));
                                manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                                manager.PushMenuScreen(overwriteDialog.menuScreen, other: file.Path);
                            }
                        })
                        .Build();
                }
            });
        }

        public static void ShowSaving(RogueMenuManager manager)
        {
            manager.PushMenuScreen(savingMenu);
        }

        public static void ShowLoading(RogueMenuManager manager)
        {
            manager.PushMenuScreen(loadingMenu);
        }

        public static void ShowDeleting(RogueMenuManager manager)
        {
            manager.PushMenuScreen(deletingMenu);
        }

        private static void LoadingCancel(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
        }

        public static void ReopenCallback(RogueMenuManager manager, string errorMsg)
        {
            if (errorMsg != null)
            {
                manager.HandleClickBack();
                ShowErrorMsg(manager, errorMsg);
                return;
            }

            manager.Reopen();
        }

        public static void ShowErrorMsg(RogueMenuManager manager, string errorMsg)
        {
            manager.AddInt(DeviceKw.StartTalk, 0);
            manager.AddObject(DeviceKw.AppendText, ":An error has occurred.");
            manager.AddObject(DeviceKw.AppendText, " (");
            manager.AddObject(DeviceKw.AppendText, errorMsg);
            manager.AddObject(DeviceKw.AppendText, ")");
            manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
            manager.PushMenuScreen(errorMsgDialog.menuScreen);
        }

        private static void ErrorMsgOK(IListMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.HandleClickBack();
        }

        private class Presenter
        {
            public Type type;
            public SelectCallback selectCallback;
            public AddCallback addCallback;

            public void Yes(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // セーブデータを上書き
                selectCallback(manager, (string)arg.Arg.Other);
            }
        }

        public enum Type
        {
            Read,
            Write
        }
    }
}
