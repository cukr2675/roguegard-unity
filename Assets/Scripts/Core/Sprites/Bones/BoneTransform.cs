using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class BoneTransform
    {
        public BoneSprite Sprite { get; }

        public Color Color { get; }

        public bool OverridesSourceColor { get; }

        public Vector3 LocalPosition { get; }

        public Quaternion LocalRotation { get; }

        /// <summary>
        /// 親ボーンと回転前の子ボーンの大きさ。
        /// </summary>
        public Vector3 ScaleOfLocalByLocal { get; }

        //public Vector3 LocalScale { get; }
        // これは扱わない。
        // 体ボーンの LocalScale の縦横の値が違うとき、子ボーンの腕を回転させると伸び縮みしてしまうため。
        // 反転には Mirror を使用する。

        /// <summary>
        /// true のとき、 Renderer を基準点とする。
        /// </summary>
        public bool TransformsInRootParent { get; }

        /// <summary>
        /// 適用するボーンを子も含めて左右反転する。 <see cref="TransformsInRootParent"/> の影響を受けない。
        /// 頭のみを左右反転しつつ耳の角度を保つために必須
        /// </summary>
        public bool LocalMirrorX { get; }

        /// <summary>
        /// 適用するボーンを子も含めて上下反転する。 <see cref="TransformsInRootParent"/> の影響を受けない。
        /// </summary>
        public bool LocalMirrorY { get; }

        public BoneTransform(
            BoneSprite sprite, Color color, bool overridesSourceColor,
            Vector3 localPosition, Quaternion localRotation, Vector3 scaleOfLocalByLocal, bool transformsInRootParent,
            bool localMirrorX, bool localMirrorY)
        {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
            ScaleOfLocalByLocal = scaleOfLocalByLocal;
            TransformsInRootParent = transformsInRootParent;
            LocalMirrorX = localMirrorX;
            LocalMirrorY = localMirrorY;
            Sprite = sprite;
            Color = color;
            OverridesSourceColor = overridesSourceColor;
        }
    }
}
