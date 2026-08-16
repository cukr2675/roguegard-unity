using System.IO;
using System.Linq;

namespace Lysionium
{
    public class FileSelectionScreen<TMgr> :
        IListuiScreen<TMgr>,
        IListuiScreen<TMgr, FileSelectionScreen<TMgr>.Context>
        where TMgr : IListuiScreenManager<TMgr>, IBackOptionProviderListuiManager<TMgr>
    {
        public string Dir => directory;
        private readonly string directory;
        private readonly string fileSearchPattern;
        private readonly string fileExtension;
        private readonly bool includeSubDirectories;
        private readonly bool createDirectoryIfNotExists;
        private readonly ScrollMenuViewData<FileSystemInfo, TMgr> view = new()
        {
        };
        private readonly System.Func<TMgr, IListHandlerSubview> dialogSubviewSelector;
        private readonly System.Func<TMgr, IMessageBoxSubview> captionBoxSubviewSelector;

        private readonly SelectOptionList<TMgr> heads = new();
        private readonly SelectOptionList<TMgr> tails = new();
        private ChoicesScreen<TMgr, Context> overwriteDialog;
        private ChoicesScreen<TMgr, Context> errorScreenOfContainsSlash;
        private ChoicesScreen<TMgr, Context> errorScreenOfDirectoryExists;
        private ChoicesScreen<TMgr, Context> errorScreenOfExternalEffect;
        private ClickItemHandler<string, TMgr> onSubmit;

        private Context context;

        public FileSelectionScreen(
            string title, string directory, string fileSearchPattern = null, bool includeSubDirectories = true, bool createDirectoryIfNotExists = true,
            System.Func<TMgr, IListHandlerSubview> scrollSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> dialogSubviewSelector = null,
            System.Func<TMgr, IMessageBoxSubview> captionBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> backAnchorSubviewSelector = null,
            bool includeDefaultBack = true)
        {
            this.directory = directory ?? throw new System.ArgumentNullException(nameof(directory));
            this.fileSearchPattern = fileSearchPattern;
            fileExtension = GetFileExtension(fileSearchPattern);
            this.includeSubDirectories = includeSubDirectories;
            this.createDirectoryIfNotExists = createDirectoryIfNotExists;

            view.Title = title;
            if (scrollSubviewSelector != null) { view.ScrollSubviewSelector = scrollSubviewSelector; }
            if (dialogSubviewSelector != null) { this.dialogSubviewSelector = dialogSubviewSelector; }
            if (captionBoxSubviewSelector != null) { view.CaptionBoxSubviewSelector = this.captionBoxSubviewSelector = captionBoxSubviewSelector; }
            if (backAnchorSubviewSelector != null) { view.BackAnchorSubviewSelector = backAnchorSubviewSelector; }
            if (!includeDefaultBack) { view.BackAnchorList.Clear(); }

            errorScreenOfContainsSlash = new ChoicesScreen<TMgr, Context>(
                (_, _) => "File name cannot contain slashes. Use a different name.").Back();
            errorScreenOfDirectoryExists = new ChoicesScreen<TMgr, Context>(
                (_, _) => "A folder with the same name exists. Use a different name.").Back();
            errorScreenOfExternalEffect = new ChoicesScreen<TMgr, Context>(
                (_, _) => "Unknown error, possibly caused by an external process.").Back();
        }

        private static string GetFileExtension(string fileSearchPattern)
        {
            var index = fileSearchPattern.LastIndexOf("*.");
            if (index >= 0) return fileSearchPattern.Substring(index + 1);
            else return null;
        }

        public FileSelectionScreen<TMgr> Head(string name, System.Action<TMgr> onClick, string style = null)
        {
            heads.Option(name, onClick, style);
            return this;
        }

        public FileSelectionScreen<TMgr> HeadDialog(
            string name, string message, string submitName, string style = null, string submitStyle = null)
        {
            var screen = new EntryDialog(this, message, submitName, submitStyle);
            heads.Option(name, screen, style);
            return this;
        }

        public FileSelectionScreen<TMgr> Tail(string name, System.Action<TMgr> onClick, string style = null)
        {
            tails.Option(name, onClick, style);
            return this;
        }

        public FileSelectionScreen<TMgr> TailDialog(
            string name, string message, string submitName, string style = null, string submitStyle = null)
        {
            var screen = new EntryDialog(this, message, submitName, submitStyle);
            tails.Option(name, screen, style);
            return this;
        }

        public FileSelectionScreen<TMgr> OverwriteDialog(
            System.Func<string, TMgr, string> getMessage, string overwriteName, string overwriteStyle = null)
        {
            if (overwriteDialog != null) throw new System.InvalidOperationException($"{nameof(OverwriteDialog)} が二回呼び出されました。");

            overwriteDialog = new ChoicesScreen<TMgr, Context>(
                (manager, context) =>
                {
                    return getMessage(context.CurrentPath, manager);
                })
                .Option(overwriteName, (manager, context) =>
                {
                    manager.BackOption.Click(manager);
                    SubmitPath(context.CurrentPath, manager, allowOverwrite: true);
                }, overwriteStyle)
                .Back();
            return this;
        }

        /// <summary>
        /// <see cref="HeadDialog"/> と <see cref="TailDialog"/> の入力に '/' が含まれる場合のエラー
        /// </summary>
        public FileSelectionScreen<TMgr> OnErrorOfContainsSlash(System.Func<string, TMgr, string> getMessage)
        {
            if (errorScreenOfContainsSlash != null) throw new System.InvalidOperationException(
                $"{nameof(OnErrorOfContainsSlash)} が二回呼び出されました。");

            errorScreenOfContainsSlash = new ChoicesScreen<TMgr, Context>(
                (manager, context) => getMessage(context.CurrentPath, manager)).Back();
            return this;
        }

        /// <summary>
        /// <see cref="HeadDialog"/> と <see cref="TailDialog"/> の入力が既存ディレクトリと重複した場合のエラー
        /// </summary>
        public FileSelectionScreen<TMgr> OnErrorOfDirectoryExists(System.Func<string, TMgr, string> getMessage)
        {
            if (errorScreenOfDirectoryExists != null) throw new System.InvalidOperationException(
                $"{nameof(OnErrorOfContainsSlash)} が二回呼び出されました。");

            errorScreenOfDirectoryExists = new ChoicesScreen<TMgr, Context>(
                (manager, context) => getMessage(context.CurrentPath, manager)).Back();
            return this;
        }

        /// <summary>
        /// この画面を展開中のファイルシステム操作によって不正な状態になった場合のエラー
        /// </summary>
        public FileSelectionScreen<TMgr> OnErrorOfExternalEffect(System.Func<string, TMgr, string> getMessage)
        {
            if (errorScreenOfExternalEffect != null) throw new System.InvalidOperationException(
                $"{nameof(OnErrorOfContainsSlash)} が二回呼び出されました。");

            errorScreenOfExternalEffect = new ChoicesScreen<TMgr, Context>(
                (manager, context) => getMessage(context.CurrentPath, manager)).Back();
            return this;
        }

        public FileSelectionScreen<TMgr> OnSubmit(ClickItemHandler<string, TMgr> onSubmit)
        {
            this.onSubmit += onSubmit;
            return this;
        }

        //public FileSelectionScreen<TMgr> BackOption(string name = null)
        //{
        //    if (name != null)
        //    {
        //        view.BackAnchorList.Option(new BackSelectOption<TMgr>(name));
        //    }
        //    else
        //    {
        //        view.BackAnchorList.Option(BackSelectOption<TMgr>.Instance);
        //    }
        //    return this;
        //}

        public FileSelectionScreen<TMgr> BackOption(string name, System.Action<TMgr> onClick, string style = null)
        {
            view.BackAnchorList.Option(name, onClick, style);
            return this;
        }

        public void OpenScreen(TMgr manager) => OpenScreen(manager, null);

        void IListuiScreen<TMgr, Context>.OpenScreen(TMgr manager, Context context) => OpenScreen(manager, context);

        private void OpenScreen(TMgr manager, Context ctx)
        {
            if (createDirectoryIfNotExists && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            context = ctx ?? new Context { CurrentDirectory = directory };

            if (!Path.GetFullPath(context.CurrentDirectory).StartsWith(Path.GetFullPath(directory)))
            {
                context.CurrentDirectory = directory;
            }

            string[] paths;
            if (fileSearchPattern != null)
            {
                paths = Directory.GetFiles(context.CurrentDirectory, fileSearchPattern);
            }
            else
            {
                paths = Directory.GetFiles(context.CurrentDirectory);
            }

            if (includeSubDirectories)
            {
                paths = Directory.GetDirectories(context.CurrentDirectory).Concat(paths).ToArray();
            }

            //paths = paths.Select(x => Path.GetRelativePath(context.CurrentDirectory, x)).ToArray();
            var infos = paths.Select(x =>
            {
                var fileInfo = new FileInfo(x);
                if (fileInfo.Exists) return (FileSystemInfo)fileInfo;
                else return new DirectoryInfo(x);
            }).ToArray();

            view.Show(infos, manager)
                ?
                .Head.OptionRange(heads)
                .Head.Option("..", m => SubmitPath("..", m))

                .NameFrom(info =>
                {
                    var path = Path.GetRelativePath(context.CurrentDirectory, info.FullName);
                    if (info is not FileInfo fileInfo) return path;

                    string size = fileInfo.Length switch
                    {
                        >= (1024 * 1024 * 1024) => $"{fileInfo.Length / (1024 * 1024 * 1024)} GB",
                        >= (1024 * 1024) => $"{fileInfo.Length / (1024 * 1024)} MB",
                        >= 1024 => $"{fileInfo.Length / 1024} KB",
                        _ => "1 KB",
                    };

                    return
                        $"{path}<line-height=-0.5em>\n" +
                        $"</line-height><size=0.5em><align=right><alpha=#C0>{size}\n" +
                        $"{info.LastWriteTime}";
                })
                .OnClick((info, manager) => SubmitPath(Path.GetRelativePath(context.CurrentDirectory, info.FullName), manager))

                .Tail.OptionRange(tails)

                .Build();
        }

        private void SubmitPath(string path, TMgr manager, bool fromEntryDialog = false, bool allowOverwrite = false)
        {
            if (fromEntryDialog && fileExtension != null && Path.GetExtension(path) != fileExtension)
            {
                path += fileExtension;
            }

            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            context.CurrentPath = path;

            if (path.Contains(Path.DirectorySeparatorChar))
            {
                manager.PushScreen(errorScreenOfContainsSlash, context);
                return;
            }

            var relativePath = Path.Combine(context.CurrentDirectory, path);
            if (Directory.Exists(relativePath))
            {
                if (fromEntryDialog)
                {
                    manager.PushScreen(errorScreenOfDirectoryExists, context);
                }
                else if (!includeSubDirectories)
                {
                    manager.PushScreen(errorScreenOfExternalEffect, context);
                }
                else
                {
                    // フォルダを開く
                    context.CurrentDirectory = relativePath;
                    manager.BackOption.Click(manager);
                    manager.PushScreen(this, context);
                }
            }
            else if (File.Exists(relativePath) && overwriteDialog != null && !allowOverwrite)
            {
                // 上書きダイアログ
                manager.PushScreen(overwriteDialog, context);
            }
            else
            {
                // 確定
                onSubmit?.Invoke(relativePath, manager);
            }
        }

        private class EntryDialog : DelegateListuiScreen<TMgr>
        {
            private readonly DialogViewData<TMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public EntryDialog(FileSelectionScreen<TMgr> screen, string message, string submitName, string submitStyle)
            {
                if (screen.dialogSubviewSelector != null) { view.DialogSubviewSelector = screen.dialogSubviewSelector; }
                if (screen.captionBoxSubviewSelector != null) { view.CaptionBoxSubviewSelector = screen.captionBoxSubviewSelector; }

                OnOpenScreen += (manager) =>
                {
                    view.Show(message, manager)
                    ?
                    .VarOnce(out var invalidFileNameChars, Path.GetInvalidFileNameChars())
                    .VarOnce(out var entryPath, "")
                    .Tail.Append(InputFieldWidgetOption.Create<TMgr>(
                        _ => entryPath,
                        value => entryPath = invalidFileNameChars.Any(x => value.Contains(x)) ? entryPath : value))

                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<TMgr>(
                            submitName, (manager) => screen.SubmitPath(entryPath, manager, fromEntryDialog: true), submitStyle)),
                        ("1*", manager.BackOption)))

                    .Build();
                };

                OnCloseScreenView += (manager, back) => view.Hide(manager, back);
            }
        }

        private class Context
        {
            public string CurrentDirectory { get; set; }

            public string CurrentPath { get; set; }
        }
    }
}
