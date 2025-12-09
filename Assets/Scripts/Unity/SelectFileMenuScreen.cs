using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;
using System.IO;

namespace RoguegardUnity
{
    internal class SelectFileMenuScreen : RogueMenuScreen
    {
        private RogueMenuScreen nextScreen;
        private ClickItemHandler<MMgr, MArg> onNewFile;
        private RogueScrollMenuViewData<object> view;
        private readonly List<FileInfo> files = new();

        private static readonly LoadingListMenuScreen savingMenu = new("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly ChoicesMenuScreen errorMsgDialog
            = new ChoicesMenuScreen((manager, arg) => $":An error has occurred.:, ({arg.Arg.Other})").Option("OK", ErrorMsgOK);

        private SelectFileMenuScreen() { }

        public static SelectFileMenuScreen Load(
            ClickItemHandler<FileInfo, MMgr, MArg> onSelectFile,
            ClickItemHandler<MMgr, MArg> onNewFile = null)
        {
            var instance = new SelectFileMenuScreen
            {
                nextScreen = new SelectFileCommandMenuScreen(onSelectFile),
                onNewFile = onNewFile
            };

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
            var instance = new SelectFileMenuScreen
            {
                nextScreen = new ChoicesMenuScreen(
                    (manager, arg) => StandardRogueDeviceUtility.LocalizeMessage(":OverwriteMsg::1", ((FileInfo)arg.Arg.Other).Name))
                    .Option(":Overwrite", (manager, arg) => onSelectFile((FileInfo)arg.Arg.Other, manager, arg))
                    .Back(),
                onNewFile = onNewFile,

                view = new()
                {
                }
            };

            return instance;
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            files.Clear();
            files.AddRange(StandardRogueDeviceSave.GetFiles());

            view.Show(files, manager, arg)
                ?
                .VarOnce(out var newArg, new MArg.Builder())

                .InitIf(
                    onNewFile != null, x => x
                    
                    .Head.Option(":+ New File", onNewFile)
                    
                    )

                .InfoFrom((item, manager, arg) =>
                {
                    if (item is FileInfo fileInfo)
                    {
                        var name = fileInfo.Name;
                        var infoText1 = $"{fileInfo.Length / 1000:N0}KB";
                        var infoText2 = fileInfo.LastWriteTime.ToString();
                        return (name, infoText1, infoText2);
                    }
                    else if (item is ISelectOption<MMgr, MArg> option)
                    {
                        var name = option.GetName(manager, arg);
                        return (name, null, null);
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                })

                .OnClick((item, manager, arg) =>
                {
                    if (item is FileInfo fileInfo)
                    {
                        newArg.Arg = new(other: fileInfo);
                        manager.PushMenuScreen(nextScreen, newArg.ReadOnly);
                    }
                    else if (item is ISelectOption<MMgr, MArg> option) { option.Click(manager, arg); }
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

        private static void ErrorMsgOK(MMgr manager, MArg arg)
        {
            manager.BackOption.Click(manager, arg);
        }

        private class ImportScreen : RogueMenuScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewSelector = m => m.Overlay,
                BackAnchorSubviewSelector = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.Show("インポート中…", manager, arg)
                    ?
                    .Tail.Option("キャンセル", (manager, arg) => manager.PopMenuScreen())

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
