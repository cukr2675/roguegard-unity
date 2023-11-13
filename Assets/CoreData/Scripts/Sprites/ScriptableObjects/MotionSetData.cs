using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/MotionSet")]
    public class MotionSetData : ScriptableObject, IMotionSet
    {
        [SerializeField] private List<Item> _items = null;

        [System.NonSerialized] private IBoneMotion firstValue;

        public bool ContainsKey(IKeyword key)
        {
            foreach (var item in _items)
            {
                if (item.Key == key) return true;
            }
            return false;
        }

        public bool TryGetValue(IKeyword key, out IBoneMotion value)
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

        void IMotionSet.GetPose(IKeyword keyword, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion)
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
            [SerializeField] private KeywordData _key;
            public IKeyword Key => _key;

            [SerializeField] private BoneMotionData _value;
            public IBoneMotion Value => _value;
        }
    }
}
