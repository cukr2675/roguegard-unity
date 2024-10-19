using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ExitListMenuSelectOption : IListMenuSelectOption
    {
        public static ExitListMenuSelectOption Instance { get; } = new ExitListMenuSelectOption();

        private static readonly object[] single = new[] { Instance };
        private static IElementsSubViewStateProvider subViewStateProvider;

        public static ExitListMenuSelectOption Create<TMgr, TArg>()
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new ExitListMenuSelectOption();
            return instance;
        }

        string IListMenuSelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            return manager.BackName;
        }

        void IListMenuSelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg)
        {
            manager.HandleClickBack();
        }

        public static void ShowBackAnchorExit(IListMenuManager manager)
        {
            manager
                .GetSubView(StandardSubViewTable.BackAnchorName)
                .Show(single, SelectOptionHandler.Instance, manager, null, ref subViewStateProvider);
        }
    }
}
