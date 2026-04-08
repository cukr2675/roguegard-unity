using Lysionium;

namespace Roguegard.Device
{
    public class CharacterCreationViewData : ListViewData<object, MMgr>
    {
        private readonly ButtonViewItemHandler<object, MMgr> subviewHandler = new();

        public SelectOptionList<MMgr> BackAnchorList { get; set; }
        public System.Func<MArg> Args { get; set; }

        private ISubviewStateProvider characterCreationStateProvider;
        private ISubviewStateProvider backAnchorStateProvider;

        public Builder Show(MMgr manager)
        {
            SetOriginalList(System.Array.Empty<object>(), manager);

            if (TryShowSubviews(manager)) return null;
            else return new Builder(this, manager);
        }

        protected override void ShowSubviews(MMgr manager)
        {
            manager.CharacterCreation.SetListHandler(List, subviewHandler, manager, Args(), ref characterCreationStateProvider);
            manager.CharacterCreation.Show(onHide: OnHide);

            manager.BackAnchor.Show(BackAnchorList, SelectOptionViewItemHandler<MMgr>.Instance, manager, ref backAnchorStateProvider);
        }

        public class Builder : BaseListBuilder<CharacterCreationViewData, Builder>
        {
            public Builder(CharacterCreationViewData parent, MMgr manager)
                : base(parent, manager)
            {
            }
        }
    }
}
