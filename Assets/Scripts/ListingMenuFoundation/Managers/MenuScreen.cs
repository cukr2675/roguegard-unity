using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// ���j���[�̉�ʒP�ʂ̃N���X
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        // ViewTemplate �N���X�̃��\�b�h�`�F�[���Ō���Ďg�p���Ȃ��悤�� in �����ɂ���
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
