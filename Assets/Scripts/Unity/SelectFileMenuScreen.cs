using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SelectFileMenuScreen : RogueMenuScreen
    {
        private RogueMenuScreen nextScreen;
        private ClickItemHandler<MMgr, MArg> onNewFile;
        private RogueScrollViewData<object> view;
        private readonly List<FileInfo> files = new();

        private static readonly LoadingListMenuScreen savingMenu = new LoadingListMenuScreen("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly ChoicesMenuScreen errorMsgDialog
            = new ChoicesMenuScreen((manager, arg) => $":An error has occurred.:, ({arg.Arg.Other})").Option("OK", ErrorMsgOK);

        private SelectFileMenuScreen() { }

        public static SelectFileMenuScreen Load(
            ClickItemHandler<FileInfo, MMgr, MArg> onSelectFile,
            ClickItemHandler<MMgr, MArg> onNewFile = null)
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
                        manager.PopMenuScreen();

                        if (errorMsg != null)
                        {
                            ShowErrorMsg(manager, errorMsg);
                            return;
                        }
                    });
                }, "Submit click:Sp1"));

            return instance;
        }

        public static SelectFileMenuScreen Save(
            ClickItemHandler<FileInfo, MMgr, MArg> onSelectFile,
            ClickItemHandler<MMgr, MArg> onNewFile = null)
        {
            var instance = new SelectFileMenuScreen();
            instance.nextScreen = new ChoicesMenuScreen(
                (manager, arg) => StandardRogueDeviceUtility.LocalizeMessage(":OverwriteMsg::1", ((FileInfo)arg.Arg.Other).Name))
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

            view.Show(files, manager, arg)
                ?
                .VarOnce(out var newArg, new MArg.Builder())

                .InitIf(
                    onNewFile != null, x => x
                    
                    .Head(SelectOption.Create(":+ New File", onNewFile))
                    
                    )

                .InfoFrom((element, manager, arg) =>
                {
                    if (element is FileInfo fileInfo)
                    {
                        var name = fileInfo.Name;
                        var infoText1 = $"{fileInfo.Length / 1000:N0}KB";
                        var infoText2 = fileInfo.LastWriteTime.ToString();
                        return (name, infoText1, infoText2);
                    }
                    else if (element is ISelectOption option)
                    {
                        var name = option.GetName(manager, arg);
                        return (name, null, null);
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                })

                .OnClick((element, manager, arg) =>
                {
                    if (element is FileInfo fileInfo)
                    {
                        newArg.Arg = new(other: fileInfo);
                        manager.PushMenuScreen(nextScreen, newArg.ReadOnly);
                    }
                    else if (element is ISelectOption option) { option.Click(manager, arg); }
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
                manager.PopMenuScreen();
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
            manager.BackOption.Click(manager, arg);
        }

        private class ImportScreen : RogueMenuScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewName = StandardSubviewTable.OverlayName,
                BackAnchorSubviewName = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show("インポート中…", manager, arg)
                    ?
                    .Option("キャンセル", (manager, arg) => manager.PopMenuScreen())

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
