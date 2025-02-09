using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public interface IReadOnlyOchalikeBone
    {
        BoneKeyword Name { get; }

        BoneSprite BareSprite { get; }

        Color BareColor { get; }

        /// <summary>
        /// true のとき、このボーンをベースカラーから上書きする。
        /// </summary>
        bool OverridesOnDefaultColor { get; }

        /// <summary>
        /// このボーンのスプライト（装備含む）のみを左右反転する。 <see cref="SpritePoseBoneTransform.LocalMirrorX"/> と違い子ボーンはそのまま。
        /// </summary>
        bool FlipX { get; }

        /// <summary>
        /// このボーンのスプライト（装備含む）のみを上下反転する。 <see cref="SpritePoseBoneTransform.LocalMirrorY"/> と違い子ボーンはそのまま。
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

        IReadOnlyList<IReadOnlyOchalikeBone> Children { get; }
    }
}
