using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    // AnimationClip ではなく BonePose を自作する理由
    // ・AnimationClip では Front と Rear それぞれ同じ位置に Transform が必要となって面倒なため
    // ・ランタイムではスプライトを変更する AnimationClip を動的生成できないため (AnimationUtility.SetEditorCurve がエディタでしか使えない)
    // ・AnimationClip で1フレーム内に複数の AnimationEvent を設定すると Animation ビュー上で重なって表示されて面倒なため

    public class BonePose
    {
        /// <summary>
        /// true のとき、この <see cref="BonePose"/> を不変として扱い最適化する。
        /// </summary>
        public bool IsImmutable { get; private set; }

        /// <summary>
        /// true のとき、この <see cref="BonePose"/> を適用した <see cref="RogueObjSprite"/> を背中向きにする。
        /// </summary>
        public bool Back { get; private set; }

        private readonly Dictionary<IKeyword, BoneTransform> _boneTransforms;
        public IReadOnlyDictionary<IKeyword, BoneTransform> BoneTransforms => _boneTransforms;

        private BoneOrder _boneOrder;
        public BoneOrder BoneOrder => _boneOrder;

        private static readonly BoneOrder defaultBoneOrder = new BoneOrder(new BoneBack[0], new BoneReorder[0]);

        public BonePose()
        {
            IsImmutable = false;
            Back = false;
            _boneTransforms = new Dictionary<IKeyword, BoneTransform>();
            _boneOrder = defaultBoneOrder;
        }

        public void SetBack(bool back)
        {
            if (IsImmutable) throw new RogueException();
            Back = back;
        }

        public void AddBoneTransform(BoneTransform value, IKeyword name)
        {
            if (IsImmutable) throw new RogueException();
            _boneTransforms.Add(name, value);
        }

        public void SetBoneOrder(BoneOrder boneOrder)
        {
            if (IsImmutable) throw new RogueException();
            _boneOrder = boneOrder;
        }

        public void SetImmutable()
        {
            if (IsImmutable) throw new RogueException();
            IsImmutable = true;
        }
    }
}
