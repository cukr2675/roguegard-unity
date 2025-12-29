using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;
using System.IO;

namespace RoguegardUnity
{
    internal class FileSelectionScreen : RogueListuiScreen
    {
        private RogueListuiScreen nextScreen;
        private ClickItemHandler<MMgr, MArg> onNewFile;
        private RogueScrollMenuViewData<object> view;
        private readonly List<FileInfo> files = new();

        private static readonly LoadingScreen savingScreen = new("セーブ中…", "キャンセル", LoadingCancel);
        private static readonly ChoicesScreen errorMsgDialogScreen
            = new ChoicesScreen((manager, arg) => $":An error has occurred.:, ({arg.Arg.Other})").Option("OK", ErrorMsgOK);

        private FileSelectionScreen() { }

        public static FileSelectionScreen Load(
            ClickItemHandler<FileInfo, MMgr, MArg> onSelectFile,
            ClickItemHandler<MMgr, MArg> onNewFile = null)
        {
            var instance = new FileSelectionScreen
            {
                nextScreen = new SelectFileCommandMenuScreen(onSelectFile),
                onNewFile = onNewFile
            };

            var importScreen = new ImportScreen();

            instance.view = new()
            {
                BackAnchorList = new(
                    _ => _
                    .Option(":Import", (manager, arg) =>
                    {
                        manager.PushScreen(importScreen);
                        RogueFile.Import(StandardRogueDeviceSave.RootDirectory, errorMsg =>
                        {
                            manager.PopScreen();

                            if (errorMsg != null)
                            {
                                ShowErrorMsg(manager, errorMsg);
                                return;
                            }
                        });
                    }, "Submit click:Sp1")
                    .Back())
            };

            return instance;
        }

        public static FileSelectionScreen Save(
            ClickItemHandler<FileInfo, MMgr, MArg> onSelectFile,
            ClickItemHandler<MMgr, MArg> onNewFile = null)
        {
            var instance = new FileSelectionScreen
            {
                nextScreen = new ChoicesScreen(
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
                        manager.PushScreen(nextScreen, newArg.ReadOnly);
                    }
                    else if (item is ISelectOption<MMgr, MArg> option) { option.Click(manager, arg); }
                    else throw new System.InvalidOperationException();
                })

                .Build();
        }

        public static void ShowSaving(MMgr manager)
        {
            manager.PushScreen(savingScreen);
        }

        private static void LoadingCancel(MMgr manager, MArg arg)
        {
        }

        public static void ReopenCallback(MMgr manager, string errorMsg)
        {
            if (errorMsg != null)
            {
                manager.PopScreen();
                ShowErrorMsg(manager, errorMsg);
                return;
            }

            manager.Reopen();
        }

        public static void ShowErrorMsg(MMgr manager, string errorMsg)
        {
            manager.PushScreen(errorMsgDialogScreen, other: errorMsg);
        }

        private static void ErrorMsgOK(MMgr manager, MArg arg)
        {
            manager.BackOption.Click(manager, arg);
        }

        private class ImportScreen : RogueListuiScreen
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
                    .Tail.Option("キャンセル", (manager, arg) => manager.PopScreen())

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
