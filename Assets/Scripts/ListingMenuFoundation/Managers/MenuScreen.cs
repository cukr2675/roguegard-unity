using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// メニューの画面単位のクラス
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg> : IMenuScreen
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        // ViewTemplate クラスのメソッドチェーンで誤って使用しないように in 引数にする
        public abstract void OpenScreen(in TMgr manager, in TArg arg);

        public virtual void CloseScreen(in TMgr manager, bool back)
        {
            manager.HideAll(back);
        }

        void IMenuScreen.OpenScreen(IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

            OpenScreen(tMgr, tArg);
        }
    }
}
