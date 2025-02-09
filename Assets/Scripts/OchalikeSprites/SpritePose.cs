using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    // AnimationClip ではなく SpritePose を自作する理由
    // ・AnimationClip では Front と Rear それぞれ同じ位置に Transform が必要となって面倒なため
    // ・ランタイムではスプライトを変更する AnimationClip を動的生成できないため (AnimationUtility.SetEditorCurve がエディタでしか使えない)
    // ・AnimationClip で1フレーム内に複数の AnimationEvent を設定すると Animation ビュー上で重なって表示されて面倒なため

    // 用途: 一時的に有効にするもの
    // 例: モーション、表情

    // 命名メモ: ISpriteMotion にならって OchalikePose ではなく SpritePose

    /// <summary>
    /// <see cref="OchalikeSpriteData"/> のポージングをするクラス。
    /// <see cref="OchalikeBone"/> を <see cref="OchalikeMorph"/> で変更を加えたところに位置・角度・スプライト・色などを変更する
    /// </summary>
    public class SpritePose
    {
        /// <summary>
        /// true のとき、この <see cref="SpritePose"/> を不変として扱い最適化する。
        /// </summary>
        public bool IsImmutable { get; private set; }

        /// <summary>
        /// true のとき、この <see cref="SpritePose"/> を適用した <see cref="OchalikeSpriteData"/> を背中向きにする。
        /// </summary>
        public bool Back { get; private set; }

        private readonly Dictionary<BoneKeyword, SpritePoseBoneTransform> _boneTransforms;
        public IReadOnlyDictionary<BoneKeyword, SpritePoseBoneTransform> BoneTransforms => _boneTransforms;

        private BoneOrder _boneOrder;
        public BoneOrder BoneOrder => _boneOrder;

        private static readonly BoneOrder defaultBoneOrder = new BoneOrder(new BoneBack[0], new BoneReorder[0]);

        public SpritePose()
        {
            IsImmutable = false;
            Back = false;
            _boneTransforms = new Dictionary<BoneKeyword, SpritePoseBoneTransform>();
            _boneOrder = defaultBoneOrder;
        }

        public void Clear()
        {
            if (IsImmutable) throw new System.Exception();

            Back = false;
            _boneTransforms.Clear();
            _boneOrder = defaultBoneOrder;
        }

        public void SetBack(bool back)
        {
            if (IsImmutable) throw new System.Exception();
            Back = back;
        }

        public void SetBoneTransforms(SpritePose pose)
        {
            if (IsImmutable) throw new System.Exception();

            foreach (var pair in pose._boneTransforms)
            {
                SetBoneTransform(pair.Value, pair.Key);
            }
        }

        public void AddBoneTransform(SpritePoseBoneTransform value, BoneKeyword name)
        {
            if (IsImmutable) throw new System.Exception();
            _boneTransforms.Add(name, value);
        }

        public void SetBoneTransform(SpritePoseBoneTransform value, BoneKeyword name)
        {
            if (IsImmutable) throw new System.Exception();
            _boneTransforms[name] = value;
        }

        public void SetBoneOrder(BoneOrder boneOrder)
        {
            if (IsImmutable) throw new System.Exception();
            _boneOrder = boneOrder;
        }

        public void SetImmutable()
        {
            if (IsImmutable) throw new System.Exception();
            IsImmutable = true;
        }
    }
}
