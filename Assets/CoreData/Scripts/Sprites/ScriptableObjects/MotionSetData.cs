using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/MotionSet")]
    public class MotionSetData : ScriptableObject, IMotionSet
    {
        [SerializeField] private List<Item> _items = null;

        [System.NonSerialized] private IBoneMotion firstValue;

        //public bool ContainsKey(IKeyword key)
        //{
        //    foreach (var item in _items)
        //    {
        //        if (item.Key == new BoneMotionKeyword(key.Name)) return true;
        //    }
        //    return false;
        //}

        public bool TryGetValue(BoneMotionKeyword key, out IBoneMotion value)
        {
            foreach (var item in _items)
            {
                if (item.Key == key)
                {
                    value = item.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        void IMotionSet.GetPose(BoneMotionKeyword keyword, int animationTime, SpriteDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion)
        {
            if (!TryGetValue(keyword, out var value))
            {
                // キーワードと一致するモーションが存在しない場合、代わりに最初のモーションを使う。
                if (firstValue == null) { firstValue = _items[0].Value; }
                value = firstValue;
            }

            value.ApplyTo(this, animationTime, direction, ref transform, out endOfMotion);
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private Roguegard.KeywordData _key;
            public BoneMotionKeyword Key => new BoneMotionKeyword(_key.DescriptionName);

            [SerializeField] private BoneMotionData _value;
            public IBoneMotion Value => _value;
        }
    }
}
