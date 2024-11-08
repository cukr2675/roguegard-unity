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
        void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        void Show(HandleEndAnimation onEndAnimation = null);

        void Hide(bool back, HandleEndAnimation onEndAnimation = null);
    }
}
