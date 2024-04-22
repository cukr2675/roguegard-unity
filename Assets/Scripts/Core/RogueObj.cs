using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueObj
    {
        public int Stack { get; private set; }

        public RogueSpace Space { get; private set; }

        public RogueObj Location { get; private set; }

        public Vector2Int Position { get; private set; }

        public bool AsTile { get; private set; }

        /// <summary>
        /// <see cref="HasCollider"/> が true のオブジェクトと衝突するかを取得する。
        /// </summary>
        public bool HasCollider { get; private set; }

        /// <summary>
        /// 固定されているオブジェクトまたはタイルと衝突するかを取得する。
        /// </summary>
        public bool HasTileCollider { get; private set; }

        public bool HasSightCollider { get; private set; }

        public MainRogueObjInfo Main { get; set; }

        private readonly Dictionary<System.Type, IRogueObjInfo> infos;

        [Objforming.CreateInstance]
        private RogueObj(bool dummy) { }

        public RogueObj()
        {
            infos = new Dictionary<System.Type, IRogueObjInfo>();
            Stack = 1;
            Space = new RogueSpace();
        }

        /// <summary>
        /// 位置と <see cref="Space"/> を除いた複製を取得する。
        /// </summary>
        public RogueObj Clone(bool excludeSpace = false)
        {
            var clone = new RogueObj();
            clone.Main = Main.Clone(this, clone);
            foreach (var pair in infos)
            {
                var copiedValue = pair.Value.DeepOrShallowCopy(this, clone);
                if (copiedValue == null) continue;

                clone.infos.Add(pair.Key, copiedValue);
            }

            if (!excludeSpace) { clone.Space = new RogueSpace(Space); }
            else { clone.Space = new RogueSpace(); }

            var spaceObjs = Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null) continue;

                // null の場合も ReplaceCloned で既存オブジェクトを null に置き換える
                RogueObj clonedSpaceObj = null;
                if (!excludeSpace)
                {
                    clonedSpaceObj = spaceObj.Clone();
                    if (!clonedSpaceObj.TryLocate(
                        clone, spaceObj.Position, spaceObj.AsTile, spaceObj.HasCollider, spaceObj.HasTileCollider, spaceObj.HasSightCollider,
                        StackOption.NotStack))
                    {
                        Debug.LogError($"{nameof(RogueObj)} の複製に失敗しました。");
                        continue;
                    }
                }

                var keys = clone.infos.Keys.ToArray();
                foreach (var pair in keys)
                {
                    var info = clone.infos[pair].ReplaceCloned(spaceObj, clonedSpaceObj);
                    if (info == null) continue;

                    clone.infos[pair] = info;
                }
                clone.Main.ReplaceCloned(spaceObj, clonedSpaceObj);
            }
            return clone;
        }

        public bool TryLocate(
            RogueObj location, Vector2Int position, bool asTile, bool collide, bool tileCollide, bool sightCollide, StackOption stackOption)
        {
            if (location == null)
            {
                Location?.Space.ReplaceWithNull(this);

                // 空間を移動したとき移動元と移動先の重量を再計算させる。
                WeightCalculator.SetDirty(Location);

                Location = null;
                Position = Vector2Int.zero;
                AsTile = asTile;
                HasCollider = collide;
                HasTileCollider = tileCollide;
                HasSightCollider = sightCollide;
                return true;
            }

            // 空間を循環参照させることはできない。（重さ計算で循環参照は無限再帰になる）
            if (location == this || Contains(location)) return false;

            // 消滅したオブジェクトのとき、さらに空間移動を設定することはできない。
            if (Stack == 0) return false;

            // 移動先のオブジェクトの個数が 1 でないとき移動失敗。
            if (location.Stack != 1) return false;

            var space = location.Space;
            if (space == null)
            {
                Debug.LogError("移動先に空間がありません。");
                return false;
            }

            // 自動スタックが有効ならスタックする。
            if (stackOption != StackOption.NotStack)
            {
                var maxStack = GetMaxStack(stackOption);
                space.Stack(this, position, maxStack);
                if (Stack == 0)
                {
                    // 空間を移動したとき移動元の重量を再計算させる。（移動物は TryStack 内で再計算される）
                    WeightCalculator.SetDirty(Location);
                    return true;
                }
            }

            // 移動を試みる。
            {
                var result = space.TryLocate(this, position, asTile, collide, tileCollide);
                if (!result) return false;
            }

            // 空間を移動したとき移動元と移動先の重量を再計算させる。
            if (Location != location)
            {
                WeightCalculator.SetDirty(Location);
                WeightCalculator.SetDirty(location);
            }

            Location = location;
            Position = position;
            AsTile = asTile;
            HasCollider = collide;
            HasTileCollider = tileCollide;
            HasSightCollider = sightCollide;
            return true;

            bool Contains(RogueObj location1)
            {
                if (location1 == null) return false;
                else if (location1.Location == this) return true;
                else return Contains(location1.Location);
            }
        }

        public bool TryLocate(Vector2Int position, bool asTile, bool collide, bool tileCollide, bool sightCollide)
        {
            return TryLocate(Location, position, asTile, collide, tileCollide, sightCollide, StackOption.NotStack);
        }

        public bool TrySetStack(int stack, RogueObj user = null)
        {
            if (stack >= 2 && Space != null && Space.Objs.Count >= 1)
            {
                // 子オブジェクトを持つオブジェクトをスタックすることはできない。
                return false;
            }

            stack = Mathf.Max(stack, 0); // 個数はマイナスにならない。

            // 個数が変わったとき重量を再計算させる。
            if (Stack != stack) { WeightCalculator.SetDirty(this); }

            Stack = stack;
            if (Stack <= 0)
            {
                // ０個になったら null 空間へ移動させて消去する。
                // 装備解除時に再スタックさせるための移動が必要なため、 activationDepth = 100 で実行する。
                const float setStackLocateActivationDepth = 100f;
                var locateMethod = Main.InfoSet.Locate;
                var arg = new RogueMethodArgument(targetObj: null);
                RogueMethodAspectState.Invoke(MainInfoKw.Locate, locateMethod, this, user, setStackLocateActivationDepth, arg);
            }
            return true;
        }

        public int GetMaxStack(StackOption stackOption)
        {
            return GetMaxStack(Main.InfoSet, stackOption);
        }

        public static int GetMaxStack(MainInfoSet infoSet, StackOption stackOption)
        {
            if (stackOption == StackOption.Default) { stackOption = StackOption.StackUntilMax; } // Default の場合は規定値に変換する。
            var baseWeight = infoSet.Weight;
            var maxStack = baseWeight == 0f ? 1 : Mathf.Max(Mathf.FloorToInt(1f / baseWeight), 1);
            maxStack = stackOption == StackOption.StackUntilMax ? maxStack : int.MaxValue;
            return maxStack;
        }

        public T Get<T>()
            where T : IRogueObjInfo
        {
            var type = typeof(T);
            if (type.IsAbstract || type.IsInterface)
            {
                Debug.LogWarning($"抽象クラスかインターフェースを {nameof(Get)} に指定しています。この値は無効です。");
            }

            return (T)infos[typeof(T)];
        }

        public bool TryGet<T>(out T info)
            where T : IRogueObjInfo
        {
            var type = typeof(T);
            if (type.IsAbstract || type.IsInterface)
            {
                Debug.LogWarning($"抽象クラスかインターフェースを {nameof(TryGet)} に指定しています。この値は無効です。");
            }

            if (infos.TryGetValue(typeof(T), out var infoObj) &&
                infoObj is T infoT)
            {
                info = infoT;
                return true;
            }
            else
            {
                info = default;
                return false;
            }
        }

        public void SetInfo(IRogueObjInfo info)
        {
            infos[info.GetType()] = info;
        }

        public bool RemoveInfo(System.Type infoType)
        {
            return infos.Remove(infoType);
        }

        /// <summary>
        /// このインスタンスに <paramref name="other"/> をスタックできるかを取得する。（<paramref name="other"/> へのスタックは保証しない）
        /// </summary>
        public bool CanStack(RogueObj other)
        {
            if (other == null) return false;

            // 子オブジェクトを持つオブジェクトをスタックすることはできない。
            if (Space.Objs.Count >= 1 || Space.Tilemap != null) return false;
            if (other.Space.Objs.Count >= 1 || Space.Tilemap != null) return false;

            if (!Main.CanStack(this, other)) return false;

            var infosCount = 0;
            foreach (var pair in infos)
            {
                if (!pair.Value.CanStack(null)) { infosCount++; }
            }
            var otherInfosCount = 0;
            foreach (var pair in other.infos)
            {
                if (!pair.Value.CanStack(null)) { otherInfosCount++; }
            }
            if (otherInfosCount != infosCount) return false;

            foreach (var pair in infos)
            {
                if (pair.Value.CanStack(null)) continue;

                // 同じキーワードの項目がないとき false を返す。
                if (!other.infos.TryGetValue(pair.Key, out var otherValue)) return false;

                // スタック不可能な項目がひとつでも存在したら false を返す。
                if (!pair.Value.CanStack(otherValue)) return false;
            }
            return true;
        }

        public void GetName(RogueNameBuilder refName)
        {
            var statusEffectState = Main.GetStatusEffectState(this);
            statusEffectState.GetEffectedName(refName, this);
        }

        public string GetName()
        {
            var refName = new RogueNameBuilder();
            GetName(refName);
            return refName.ToString();
        }

        // 無限再帰対策で MainInfoSet.Name を直接取得する
        public override string ToString() => $"RogueObj[{Main?.InfoSet?.Name}]";
    }
}
