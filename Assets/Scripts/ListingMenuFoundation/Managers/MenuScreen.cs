using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// メニューの画面単位のクラス
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        // ViewTemplate クラスのメソッドチェーンで誤って使用しないように in 引数にする
        public abstract void OpenScreen(in TMgr manager, in TArg arg);

        public virtual void CloseScreen(TMgr manager, bool back)
        {
            manager.HideAll(back);
        }

        public static implicit operator HandleClickElement<TMgr, TArg>(MenuScreen<TMgr, TArg> menuScreen)
        {
            return (manager, arg) => manager.PushMenuScreenFromExtension(menuScreen, arg);
        }
    }
}
