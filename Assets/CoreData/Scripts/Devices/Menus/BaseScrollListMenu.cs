using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    //public abstract class BaseScrollListMenu<T> : IListMenu, IElementPresenter
    //{
    //    protected virtual string MenuName => null;
    //    protected virtual IKeyword ViewKeyword => DeviceKw.MenuScroll;

    //    public ElementsViewPosition ViewPosition { get; }

    //    private readonly ExitListMenuSelectOption exitSelectOption;
    //    private readonly List<object> leftAnchorList;

    //    protected BaseScrollListMenu()
    //    {
    //        ViewPosition = new ElementsViewPosition(ViewKeyword);
    //        exitSelectOption = new ExitListMenuSelectOption(Exit);
    //        leftAnchorList = new List<object>();
    //    }

    //    protected abstract Spanning<T> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

    //    public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
    //    {
    //        var list = GetList(manager, self, user, arg);
    //        var scroll = manager.GetView(ViewKeyword);
    //        scroll.OpenView(this, list, manager, self, user, arg);

    //        var holder = GetViewPositionHolder(manager, self, user, arg);
    //        if (!ViewPosition.Load(manager, holder))
    //        {
    //            var defaultPosition = GetDefaultViewPosition(manager, self, user, arg);
    //            ViewPosition.Set(manager, holder, defaultPosition);
    //        }

    //        var caption = manager.GetView(DeviceKw.MenuCaption);
    //        caption.OpenView(null, Spanning<object>.Empty, null, null, null, new(other: MenuName));

    //        leftAnchorList.Clear();
    //        leftAnchorList.Add(exitSelectOption);
    //        GetLeftAnchorList(manager, self, user, arg, leftAnchorList);
    //        var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
    //        leftAnchor.OpenView(SelectOptionPresenter.Instance, leftAnchorList, manager, self, user, arg);
    //    }

    //    protected virtual object GetViewPositionHolder(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
    //    {
    //        return self;
    //    }

    //    protected virtual float GetDefaultViewPosition(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
    //    {
    //        return 0f;
    //    }

    //    protected virtual void GetLeftAnchorList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, List<object> list)
    //    {
    //    }

    //    protected abstract string GetItemName(T element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    //    protected abstract void ActivateItem(T element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

    //    private void Exit(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
    //    {
    //        // スクロール位置を記憶する
    //        var holder = GetViewPositionHolder(manager, self, user, arg);
    //        ViewPosition.Save(manager, holder);
    //    }



    //    string IElementPresenter.GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
    //    {
    //        if (element != null && !(element is T t)) return null;

    //        // 個別処理を実行
    //        return GetItemName((T)element, manager, self, user, arg);
    //    }

    //    void IElementPresenter.ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
    //    {
    //        // スクロール位置を記憶する
    //        var holder = GetViewPositionHolder(manager, self, user, arg);
    //        ViewPosition.Save(manager, holder);

    //        if (element != null && !(element is T t))
    //        {
    //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
    //            return;
    //        }
    //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

    //        // 個別処理を実行
    //        ActivateItem((T)element, manager, self, user, arg);
    //    }
    //}
}
