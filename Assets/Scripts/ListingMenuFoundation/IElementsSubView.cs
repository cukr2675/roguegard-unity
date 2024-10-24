using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// �ݒ肳�ꂽ <see cref="IElementHandler"/> �ƃ��X�g�����Ƃ� UI ��\������C���^�[�t�F�[�X�B
    /// </summary>
    public interface IElementsSubView
    {
        /// <summary>
        /// ���̃v���p�e�B�� true �̂Ƃ� <see cref="IListMenuManager"/> �̓�����~������B�A�j���[�V������ҋ@�����邽�߂Ɏg�p����
        /// </summary>
        bool HasManagerLock { get; }

        void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        void Show(ElementsSubView.HandleEndAnimation handleEndAnimation = null);

        void Hide(bool back, ElementsSubView.HandleEndAnimation handleEndAnimation = null);
    }
}
