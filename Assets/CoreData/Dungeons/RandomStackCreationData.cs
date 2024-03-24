using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/RandomStack")]
    [Objforming.IgnoreRequireRelationalComponent]
    public class RandomStackCreationData : ScriptableCharacterCreationData
    {
        [SerializeField] private ScriptableStartingItemList _items = null;

        [SerializeField] private int minDeltaStack = 0;

        protected override bool HasNotInfoSet => true;

        // Intrinsic や Stack を無効にする
        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = WeightedRogueObjGeneratorUtility.CreateObj(_items, location, position, random, stackOption);

            // スタック上限を超えないように、元のスタック数より大きくならないようにする。
            // 生成したオブジェクトが消えないように、スタック数は 1 を下回らない。
            var minStack = Mathf.Max(obj.Stack + minDeltaStack, 1);
            var stack = random.Next(minStack, obj.Stack);
            obj.TrySetStack(stack);

            return obj;
        }

        protected override void OnValidate()
        {
            minDeltaStack = Mathf.Clamp(minDeltaStack, int.MinValue, 0);

            base.OnValidate();
        }

        protected override void GetCost(out float cost, out bool costIsUnknown)
        {
            cost = 0f;
            costIsUnknown = true;
        }
    }
}
