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
        /// true のとき、このボーンをベースカラーから上書きする。
        /// </summary>
        bool OverridesBaseColor { get; }

        /// <summary>
        /// このボーンのスプライト（装備含む）のみを左右反転する。 <see cref="BoneTransform.LocalMirrorX"/> と違い子ボーンはそのまま。
        /// </summary>
        bool FlipX { get; }

        /// <summary>
        /// このボーンのスプライト（装備含む）のみを上下反転する。 <see cref="BoneTransform.LocalMirrorY"/> と違い子ボーンはそのまま。
        /// </summary>
        bool FlipY { get; }

        Vector3 LocalPosition { get; }

        Quaternion LocalRotation { get; }

        /// <summary>
        /// 親ボーンと回転前の子ボーンの大きさ。
        /// <see cref="Transform.localScale"/> と違い回転後の子ボーンを拡縮しない。
        /// </summary>
        Vector3 ScaleOfLocalByLocal { get; }

        float NormalOrderInParent { get; }

        float BackOrderInParent { get; }

        IReadOnlyList<IReadOnlyNodeBone> Children { get; }
    }
}
