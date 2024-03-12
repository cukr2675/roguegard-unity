using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    [System.Serializable]
    public class BoneSprite
    {
        [SerializeField] private Sprite _normalFront;
        [SerializeField] private Sprite _normalRear;
        [SerializeField] private Sprite _backFront;
        [SerializeField] private Sprite _backRear;

        public Sprite NormalFront => _normalFront;
        public Sprite NormalRear => _normalRear;
        public Sprite BackFront => _backFront;
        public Sprite BackRear => _backRear;

        public BoneSprite(Sprite normalFront, Sprite normalRear, Sprite backFront, Sprite backRear)
        {
            _normalFront = normalFront;
            _normalRear = normalRear;
            _backFront = backFront;
            _backRear = backRear;
        }

        public Sprite GetFrontSprite(bool back)
        {
            if (back) return _backFront;
            else return _normalFront;
        }

        public Sprite GetRearSprite(bool back)
        {
            if (back) return _backRear;
            else return _normalRear;
        }



        public static BoneSprite CreateNF(Sprite nfSprite)
            => new BoneSprite(nfSprite, null, null, null);

        public static BoneSprite CreateNFBF(Sprite nfbfSprite)
            => new BoneSprite(nfbfSprite, null, nfbfSprite, null);

        public static BoneSprite CreateNFBR(Sprite nfbrSprite)
            => new BoneSprite(nfbrSprite, null, null, nfbrSprite);

        public static BoneSprite CreateNR(Sprite nrSprite)
            => new BoneSprite(null, nrSprite, null, null);

        public static BoneSprite CreateNRBF(Sprite nrbfSprite)
            => new BoneSprite(null, nrbfSprite, nrbfSprite, null);

        public static BoneSprite CreateNRBR(Sprite nrbrSprite)
            => new BoneSprite(null, nrbrSprite, null, nrbrSprite);

        public static BoneSprite CreateBF(Sprite bfSprite)
            => new BoneSprite(null, null, bfSprite, null);

        public static BoneSprite CreateBR(Sprite brSprite)
            => new BoneSprite(null, null, null, brSprite);



        public static BoneSprite CreateNF_NR(Sprite nfSprite, Sprite nrSprite)
            => new BoneSprite(nfSprite, nrSprite, null, null);

        public static BoneSprite CreateNFBF_NR(Sprite nfbfSprite, Sprite nrSprite)
            => new BoneSprite(nfbfSprite, nrSprite, nfbfSprite, null);

        public static BoneSprite CreateNFBR_NR(Sprite nfbrSprite, Sprite nrSprite)
            => new BoneSprite(nfbrSprite, nrSprite, null, nfbrSprite);

        public static BoneSprite CreateBF_NR(Sprite bfSprite, Sprite nrSprite)
            => new BoneSprite(null, nrSprite, bfSprite, null);

        public static BoneSprite CreateBR_NR(Sprite brSprite, Sprite nrSprite)
            => new BoneSprite(null, nrSprite, null, brSprite);



        public static BoneSprite CreateNF_NRBF(Sprite nfSprite, Sprite nrbfSprite)
            => new BoneSprite(nfSprite, nrbfSprite, nrbfSprite, null);

        public static BoneSprite CreateNFBR_NRBF(Sprite nfbrSprite, Sprite nrbfSprite)
            => new BoneSprite(nfbrSprite, nrbfSprite, nrbfSprite, nfbrSprite);

        public static BoneSprite CreateBR_NRBF(Sprite brSprite, Sprite nrbfSprite)
            => new BoneSprite(null, nrbfSprite, nrbfSprite, brSprite);



        public static BoneSprite CreateNF_NRBR(Sprite nfSprite, Sprite nrbrSprite)
            => new BoneSprite(nfSprite, nrbrSprite, null, nrbrSprite);

        public static BoneSprite CreateNFBF_NRBR(Sprite nfbfSprite, Sprite nrbrSprite)
            => new BoneSprite(nfbfSprite, nrbrSprite, nfbfSprite, nrbrSprite);

        public static BoneSprite CreateBF_NRBR(Sprite bfSprite, Sprite nrbrSprite)
            => new BoneSprite(null, nrbrSprite, bfSprite, nrbrSprite);



        public static BoneSprite CreateNF_BF(Sprite nfSprite, Sprite bfSprite)
            => new BoneSprite(nfSprite, null, bfSprite, null);

        public static BoneSprite CreateNFBR_BF(Sprite nfbrSprite, Sprite bfSprite)
            => new BoneSprite(nfbrSprite, null, bfSprite, nfbrSprite);

        public static BoneSprite CreateBR_BF(Sprite brSprite, Sprite bfSprite)
            => new BoneSprite(null, null, bfSprite, brSprite);



        public static BoneSprite CreateNF_BR(Sprite nfSprite, Sprite brSprite)
            => new BoneSprite(nfSprite, null, null, brSprite);

        public static BoneSprite CreateNFBF_BR(Sprite nfbfSprite, Sprite brSprite)
            => new BoneSprite(nfbfSprite, null, nfbfSprite, brSprite);



        public Sprite GetRepresentativeSprite()
        {
            if (_normalFront != null) return _normalFront;
            else if (_normalRear != null) return _normalRear;
            else if (_backFront != null) return _backFront;
            else return _backRear;
        }
    }
}
