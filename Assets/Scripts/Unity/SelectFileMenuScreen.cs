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
        private HandleClickElement<MMgr, MArg> onNewFile;
        private SelectFileMenuViewTemplate view;
        private readonly List<FileInfo> files = new();

        private static readonly LoadingListMenu savingMenu = new LoadingListMenu("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly ChoicesMenuScreen errorMsgDialog
            = new ChoicesMenuScreen((manager, arg) => $":An error has occurred.:, ({arg.Arg.Other})").Option("OK", ErrorMsgOK);

        private SelectFileMenuScreen() { }

        public static SelectFileMenuScreen Load(
            HandleClickElement<FileInfo, MMgr, MArg> onSelectFile,
            HandleClickElement<MMgr, MArg> onNewFile = null)
        {
            var instance = new SelectFileMenuScreen();
            instance.nextScreen = new SelectFileCommandMenuScreen(onSelectFile);
            instance.onNewFile = onNewFile;

            var importScreen = new ImportScreen();

            instance.view = new()
            {
            };
            instance.view.BackAnchorList.Insert(0,
                SelectOption.Create<MMgr, MArg>(":Import", (manager, arg) =>
                {
                    manager.PushMenuScreen(importScreen);
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
            HandleClickElement<FileInfo, MMgr, MArg> onSelectFile,
            HandleClickElement<MMgr, MArg> onNewFile = null)
        {
            var instance = new SelectFileMenuScreen();
            instance.nextScreen = new ChoicesMenuScreen(
                (manager, arg) => $":OverwriteMsg::1::{((FileInfo)arg.Arg.Other).Name}<link=\"VerticalArrow\"></link>")
                .Option(":Overwrite", (manager, arg) => onSelectFile((FileInfo)arg.Arg.Other, manager, arg))
                .Back();
            instance.onNewFile = onNewFile;

            instance.view = new()
            {
            };

            return instance;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            files.Clear();
            files.AddRange(StandardRogueDeviceSave.GetFiles());

            view.ShowTemplate(files, manager, arg)
                ?
                .VariableOnce(out var newArg, new MArg.Builder())

                .IfOnce(
                    onNewFile != null, x => x
                    
                    .InsertNext(SelectOption.Create(":+ New File", onNewFile))
                    
                    )

                .OnClickElement((element, manager, arg) =>
                {
                    if (element is FileInfo fileInfo)
                    {
                        newArg.Arg = new(other: fileInfo);
                        manager.PushMenuScreen(nextScreen, newArg.ReadOnly);
                    }
                    else if (element is ISelectOption option) { option.HandleClick(manager, arg); }
                    else throw new System.InvalidOperationException();
                })

                .Build();
        }

        public static void ShowSaving(MMgr manager)
        {
            manager.PushMenuScreen(savingMenu);
        }

        private static void LoadingCancel(MMgr manager, MArg arg)
        {
        }

        public static void ReopenCallback(MMgr manager, string errorMsg)
        {
            if (errorMsg != null)
            {
                manager.Back();
                ShowErrorMsg(manager, errorMsg);
                return;
            }

            manager.Reopen();
        }

        public static void ShowErrorMsg(MMgr manager, string errorMsg)
        {
            manager.PushMenuScreen(errorMsgDialog, other: errorMsg);
        }

        private static void ErrorMsgOK(IListMenuManager manager, MArg arg)
        {
            manager.BackOption.HandleClick(manager, arg);
        }

        private class ImportScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.OverlayName,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate("インポート中…", manager, arg)
                    ?
                    .AppendSelectOption("キャンセル", (manager, arg) => manager.Back())

                    .Build();
            }

            public override void CloseScreen(MMgr manager, bool back)
            {
                view.HideTemplate(manager, back);
            }
        }
    }
}
