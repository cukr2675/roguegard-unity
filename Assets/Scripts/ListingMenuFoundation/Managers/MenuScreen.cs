using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// ���j���[�̉�ʒP�ʂ̃N���X
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg> : IMenuScreen
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        // ViewTemplate �N���X�̃��\�b�h�`�F�[���Ō���Ďg�p���Ȃ��悤�� in �����ɂ���
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
