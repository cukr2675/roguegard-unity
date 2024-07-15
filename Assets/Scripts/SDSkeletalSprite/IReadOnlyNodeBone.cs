using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public interface IReadOnlyNodeBone
    {
        BoneKeyword Name { get; }

        BoneSprite Sprite { get; }

        Color Color { get; }

        /// <summary>
        /// true �̂Ƃ��A���̃{�[�����x�[�X�J���[����㏑������B
        /// </summary>
        bool OverridesBaseColor { get; }

        /// <summary>
        /// ���̃{�[���̃X�v���C�g�i�����܂ށj�݂̂����E���]����B <see cref="BoneTransform.LocalMirrorX"/> �ƈႢ�q�{�[���͂��̂܂܁B
        /// </summary>
        bool FlipX { get; }

        /// <summary>
        /// ���̃{�[���̃X�v���C�g�i�����܂ށj�݂̂��㉺���]����B <see cref="BoneTransform.LocalMirrorY"/> �ƈႢ�q�{�[���͂��̂܂܁B
        /// </summary>
        bool FlipY { get; }

        Vector3 LocalPosition { get; }

        Quaternion LocalRotation { get; }

        /// <summary>
        /// �e�{�[���Ɖ�]�O�̎q�{�[���̑傫���B
        /// <see cref="Transform.localScale"/> �ƈႢ��]��̎q�{�[�����g�k���Ȃ��B
        /// </summary>
        Vector3 ScaleOfLocalByLocal { get; }

        float NormalOrderInParent { get; }

        float BackOrderInParent { get; }

        IReadOnlyList<IReadOnlyNodeBone> Children { get; }
    }
}
