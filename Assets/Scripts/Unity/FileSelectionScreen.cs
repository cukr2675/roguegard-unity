using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;
using System.IO;

namespace RoguegardUnity
{
    internal class FileSelectionScreen : RogueListuiScreen
    {
        private RogueListuiScreen nextScreen;
        private ClickOptionHandler<MMgr> onNewFile;
        private RogueScrollMenuViewData<object> view;
        private readonly List<FileInfo> files = new();
        private readonly ChoicesScreen errorMsgDialogScreen;

        private static readonly LoadingScreen savingScreen = new("セーブ中…", "キャンセル", LoadingCancel);

        private FileSelectionScreen()
        {
            errorMsgDialogScreen = new ChoicesScreen((manager) => $":An error has occurred.:, ({Arg.Arg.Other})").Option("OK", ErrorMsgOK);

            OnOpenScreen += (manager) =>
            {
                files.Clear();
                files.AddRange(StandardRogueDeviceSave.GetFiles());

                view.Show(files, manager)
                ?
                .VarOnce(out var newArg, new MArg.Builder())

                .InitIf(
                    onNewFile != null, x => x
                    
                    .Head.Option(":+ New File", onNewFile)
                    
                    )

                .InfoFrom((item, manager) =>
                {
                    if (item is FileInfo fileInfo)
                    {
                        var name = fileInfo.Name;
                        var infoText1 = $"{fileInfo.Length / 1000:N0}KB";
                        var infoText2 = fileInfo.LastWriteTime.ToString();
                        return (name, infoText1, infoText2);
                    }
                    else if (item is ISelectOption<MMgr> option)
                    {
                        var name = option.GetName(manager);
                        return (name, null, null);
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                })

                .OnClick((item, manager) =>
                {
                    if (item is FileInfo fileInfo)
                    {
                        newArg.Arg = new(other: fileInfo);
                        manager.PushScreen(nextScreen, newArg.ReadOnly);
                    }
                    else if (item is ISelectOption<MMgr> option) { option.Click(manager); }
                    else throw new System.InvalidOperationException();
                })

                .Build();
            };
        }

        public static FileSelectionScreen Load(
            ClickItemHandler<FileInfo, MMgr> onSelectFile,
            ClickOptionHandler<MMgr> onNewFile = null)
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
                    .Option(":Import", (manager) =>
                    {
                        manager.PushScreen(importScreen);
                        RogueFile.Import(StandardRogueDeviceSave.RootDirectory, errorMsg =>
                        {
                            manager.PopScreen();

                            if (errorMsg != null)
                            {
                                instance.ShowErrorMsg(manager, errorMsg);
                                return;
                            }
                        });
                    }, "Submit click:Sp1")
                    .Back())
            };

            return instance;
        }

        public static FileSelectionScreen Save(
            ClickItemHandler<FileInfo, MMgr> onSelectFile,
            ClickOptionHandler<MMgr> onNewFile = null)
        {
            var instance = new FileSelectionScreen();
            instance.nextScreen = new ChoicesScreen(
                (manager) => StandardRogueDeviceUtility.LocalizeMessage(":OverwriteMsg::1", ((FileInfo)instance.Arg.Arg.Other).Name))
                .Option(":Overwrite", (manager) => onSelectFile((FileInfo)instance.Arg.Arg.Other, manager))
                .Back();
            instance.onNewFile = onNewFile;
            instance.view = new()
            {
            };

            return instance;
        }

        public static void ShowSaving(MMgr manager)
        {
            manager.PushScreen(savingScreen);
        }

        private static void LoadingCancel(MMgr manager)
        {
        }

        public void ReopenCallback(MMgr manager, string errorMsg)
        {
            if (errorMsg != null)
            {
                manager.PopScreen();
                ShowErrorMsg(manager, errorMsg);
                return;
            }

            manager.Reopen();
        }

        public void ShowErrorMsg(MMgr manager, string errorMsg)
        {
            manager.PushScreen(errorMsgDialogScreen, other: errorMsg);
        }

        private static void ErrorMsgOK(MMgr manager)
        {
            manager.BackOption.Click(manager);
        }

        private class ImportScreen : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                DialogSubviewSelector = m => m.Overlay,
                BackAnchorSubviewSelector = null,
            };

            public ImportScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show("インポート中…", manager)
                    ?
                    .Tail.Option("キャンセル", m => m.PopScreen())

                    .Build();
                };

                OnCloseScreenView += (manager, back) =>
                {
                    view.Hide(manager, back);
                };
            }
        }
    }
}
