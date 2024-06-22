using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// ���X�g�����r���[�B
    /// �ݒ肳�ꂽ <see cref="IElementPresenter"/> �ƃ��X�g�����Ƃ� UI ��\������C���^�[�t�F�[�X�B
    /// </summary>
    public interface IElementsView
    {
        void OpenView<T>(
            IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetPosition();

        void SetPosition(float position);
    }
}
