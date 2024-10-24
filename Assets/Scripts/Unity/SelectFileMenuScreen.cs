using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileMenuScreen : RogueMenuScreen
    {
        private RogueMenuScreen nextScreen;
        private HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> onNewFile;
        private SelectFileMenuViewTemplate view;
        private readonly List<FileInfo> files = new();

        private static readonly LoadingListMenu savingMenu = new LoadingListMenu("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly ChoicesMenuScreen errorMsgDialog
            = new ChoicesMenuScreen((manager, arg) => $":An error has occurred.:, ({arg.Arg.Other})").Option("OK", ErrorMsgOK);

        private SelectFileMenuScreen() { }

        public static SelectFileMenuScreen Load(
            HandleClickElement<FileInfo, RogueMenuManager, ReadOnlyMenuArg> onSelectFile,
            HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> onNewFile = null)
        {
            var instance = new SelectFileMenuScreen();
            instance.nextScreen = new SelectFileCommandMenu(onSelectFile);
            instance.onNewFile = onNewFile;

            instance.view = new()
            {
            };
            instance.view.BackAnchorList.Insert(0,
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
                }));

            return instance;
        }

        public static SelectFileMenuScreen Save(
            HandleClickElement<FileInfo, RogueMenuManager, ReadOnlyMenuArg> onSelectFile,
            HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> onNewFile = null)
        {
            var instance = new SelectFileMenuScreen();
            instance.nextScreen = new ChoicesMenuScreen(
                (manager, arg) => $":OverwriteMsg::1::{((FileInfo)arg.Arg.Other).Name}")
                .Option(":Overwrite", (manager, arg) => onSelectFile((FileInfo)arg.Arg.Other, manager, arg))
                .Exit();
            instance.onNewFile = onNewFile;

            instance.view = new()
            {
            };

            return instance;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            files.Clear();
            files.AddRange(StandardRogueDeviceSave.GetFiles());

            view.Show(files, manager, arg)
                ?
                .VariableOnce(out var newArg, new MenuArg())

                .IfOnce(
                    onNewFile != null, x => x
                    
                    .InsertFirst(ListMenuSelectOption.Create(":+ New File", onNewFile))
                    
                    )

                .OnClickElement((element, manager, arg) =>
                {
                    if (element is FileInfo fileInfo)
                    {
                        newArg.Arg = new(other: fileInfo);
                        manager.PushMenuScreen(nextScreen, newArg.ReadOnly);
                    }
                    else if (element is IListMenuSelectOption option) { option.HandleClick(manager, arg); }
                    else throw new System.InvalidOperationException();
                })

                .Build();
        }

        public static void ShowSaving(RogueMenuManager manager)
        {
            manager.PushMenuScreen(savingMenu);
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
            manager.PushMenuScreen(errorMsgDialog, other: errorMsg);
        }

        private static void ErrorMsgOK(IListMenuManager manager, ReadOnlyMenuArg arg)
        {
            manager.HandleClickBack();
        }
    }
}
