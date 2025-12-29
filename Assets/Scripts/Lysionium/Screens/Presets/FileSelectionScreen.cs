using System.IO;
using System.Linq;

namespace Lysionium
{
    /// <inheritdoc/>
    public class FileSelectionScreen<TMgr> : FileSelectionScreen<TMgr, IListuiArg>
        where TMgr : IListuiManager
    {
        public FileSelectionScreen(
            string title, string directory, string fileSearchPattern = null, bool includeSubDirectories = false, bool createDirectoryIfNotExists = true,
            System.Func<TMgr, IListHandlerSubview> scrollSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> captionBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> backAnchorSubviewSelector = null,
            bool includeDefaultBack = true)
            : base(title, directory, fileSearchPattern, includeSubDirectories, createDirectoryIfNotExists,
                  scrollSubviewSelector, captionBoxSubviewSelector, backAnchorSubviewSelector, includeDefaultBack)
        {
        }
    }

    public class FileSelectionScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        private readonly string directory;
        private readonly string fileSearchPattern;
        private readonly bool includeSubDirectories;
        private readonly bool createDirectoryIfNotExists;
        private readonly ScrollMenuViewData<string, TMgr, TArg> view = new()
        {
        };

        private readonly SelectOptionList<TMgr, TArg> heads = new();
        private readonly SelectOptionList<TMgr, TArg> tails = new();
        private ClickItemHandler<string, TMgr, TArg> onClick;

        public FileSelectionScreen(
            string title, string directory, string fileSearchPattern = null, bool includeSubDirectories = false, bool createDirectoryIfNotExists = true,
            System.Func<TMgr, IListHandlerSubview> scrollSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> captionBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> backAnchorSubviewSelector = null,
            bool includeDefaultBack = true)
        {
            this.directory = directory ?? throw new System.ArgumentNullException(nameof(directory));
            this.fileSearchPattern = fileSearchPattern;
            this.includeSubDirectories = includeSubDirectories;
            this.createDirectoryIfNotExists = createDirectoryIfNotExists;

            view.Title = title;
            if (scrollSubviewSelector != null) { view.ScrollSubviewSelector = scrollSubviewSelector; }
            if (captionBoxSubviewSelector != null) { view.CaptionBoxSubviewSelector = captionBoxSubviewSelector; }
            if (backAnchorSubviewSelector != null) { view.BackAnchorSubviewSelector = backAnchorSubviewSelector; }
            if (!includeDefaultBack) { view.BackAnchorList.Clear(); }
        }

        public FileSelectionScreen<TMgr, TArg> Head(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
        {
            heads.Option(name, onClick, style);
            return this;
        }

        public FileSelectionScreen<TMgr, TArg> Tail(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
        {
            tails.Option(name, onClick, style);
            return this;
        }

        public FileSelectionScreen<TMgr, TArg> OnClick(ClickItemHandler<string, TMgr, TArg> onClick)
        {
            this.onClick += onClick;
            return this;
        }

        //public FileSelectionScreen<TMgr, TArg> BackOption(string name = null)
        //{
        //    if (name != null)
        //    {
        //        view.BackAnchorList.Add(BackSelectOption.Create<TMgr, TArg>(name));
        //    }
        //    else
        //    {
        //        view.BackAnchorList.Add(BackSelectOption.Instance);
        //    }
        //    return this;
        //}

        public FileSelectionScreen<TMgr, TArg> BackOption(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
        {
            view.BackAnchorList.Option(name, onClick, style);
            return this;
        }

        public void OpenScreen(TMgr manager, TArg arg)
        {
            if (createDirectoryIfNotExists && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string[] paths;
            if (fileSearchPattern != null)
            {
                paths = Directory.GetFiles(directory, fileSearchPattern);
            }
            else
            {
                paths = Directory.GetFiles(directory);
            }

            if (includeSubDirectories)
            {
                paths = paths.Concat(Directory.GetDirectories(directory)).ToArray();
            }

            view.Show(paths, manager, arg)
                ?
                .Head.OptionRange(heads)

                .NameFrom(path => path)
                .OnClick(onClick)

                .Tail.OptionRange(tails)

                .Build();
        }
    }
}
