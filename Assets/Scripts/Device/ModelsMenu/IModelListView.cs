using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// ���f�����X�g�p�r���[�B
    /// �ݒ肳�ꂽ <see cref="IModelListPresenter"/> �ƃ��f�����X�g�����Ƃ� UI ��\������C���^�[�t�F�[�X�B
    /// </summary>
    public interface IModelListView
    {
        void OpenView<T>(
            IModelListPresenter presenter, Spanning<T> modelList, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetPosition();

        void SetPosition(float position);
    }
}
