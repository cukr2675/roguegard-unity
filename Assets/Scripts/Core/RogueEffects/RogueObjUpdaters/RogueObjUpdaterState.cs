using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueObjUpdaterState
    {
        private readonly List<IRogueObjUpdater> updaters;

        private static readonly StaticInitializable<IndexManager> indexManager = new StaticInitializable<IndexManager>(() => new IndexManager());

        public RogueObjUpdaterState()
        {
            updaters = new List<IRogueObjUpdater>();
        }

        public void AddFromInfoSet(RogueObj self, IRogueObjUpdater updater)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (updater == null) throw new System.ArgumentNullException(nameof(updater));

            for (int i = 0; i < updaters.Count; i++)
            {
                // 同じ Order の要素が存在するときその手前に追加する。
                if (updaters[i] != null && updaters[i].Order >= updater.Order)
                {
                    updaters.Insert(i, updater);
                    indexManager.Value.InsertIndex(i);
                    return;
                }
            }
            updaters.Add(updater);
        }

        public void AddFromRogueEffect(RogueObj self, IRogueObjUpdater updater)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (updater == null) throw new System.ArgumentNullException(nameof(updater));

            for (int i = updaters.Count - 1; i >= 0; i--)
            {
                // 同じ Order の要素が存在するときその後ろに追加する。
                if (updaters[i] != null && updaters[i].Order <= updater.Order)
                {
                    updaters.Insert(i + 1, updater);
                    indexManager.Value.InsertIndex(i + 1);
                    return;
                }
            }
            updaters.Insert(0, updater);
            indexManager.Value.InsertIndex(0);
        }

        public bool ReplaceWithNull(IRogueObjUpdater updater)
        {
            var index = updaters.IndexOf(updater);
            if (index < 0) return false;

            updaters[index] = null;
            return true;
        }

        public RogueObjUpdaterContinueType UpdateObjAndRemoveNull(RogueObj self, float activationDepth)
        {
            indexManager.Value.PushOrGetPeekIndex(activationDepth, out var updaterIndex, out var sectionIndex);
            while (updaterIndex < updaters.Count)
            {
                var updater = updaters[updaterIndex];
                if (updater == null)
                {
                    // null なら削除して次へ
                    updaters.RemoveAt(updaterIndex);
                    sectionIndex = 0;
                    continue;
                }

                var continueType = updater.UpdateObj(self, activationDepth, ref sectionIndex);
                if (continueType == RogueObjUpdaterContinueType.Continue)
                {
                    indexManager.Value.SetPeekIndex(updaterIndex, sectionIndex);
                }
                else
                {
                    indexManager.Value.SetPeekIndex(updaterIndex + 1, 0);
                }
                return RogueObjUpdaterContinueType.Continue;
            }
            indexManager.Value.Pop();
            return RogueObjUpdaterContinueType.Break;
        }

        private class IndexManager
        {
            private readonly List<Item> items = new List<Item>();

            public void PushOrGetPeekIndex(float activationDepth, out int updaterIndex, out int sectionIndex)
            {
                Item item;
                if (items.Count == 0)
                {
                    item = new Item(activationDepth);
                    items.Add(item);
                }
                else
                {
                    item = items[items.Count - 1];
                    if (activationDepth < item.ActivationDepth)
                    {
                        throw new RogueException();
                    }
                    else if (activationDepth == item.ActivationDepth)
                    {
                    }
                    else
                    {
                        item = new Item(activationDepth);
                        items.Add(item);
                    }
                }
                updaterIndex = item.UpdaterIndex;
                sectionIndex = item.SectionIndex;
            }

            public void SetPeekIndex(int updaterIndex, int sectionIndex)
            {
                var item = items[items.Count - 1];
                item.UpdaterIndex = updaterIndex;
                item.SectionIndex = sectionIndex;
                items[items.Count - 1] = item;
            }

            public void Pop()
            {
                items.RemoveAt(items.Count - 1);
            }

            public void InsertIndex(int index)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.UpdaterIndex < index) continue;

                    item.UpdaterIndex++;
                    items[i] = item;
                }
            }

            public void Clear()
            {
                items.Clear();
            }

            private struct Item
            {
                public readonly float ActivationDepth;
                public int UpdaterIndex;
                public int SectionIndex;

                public Item(float activationDepth)
                {
                    ActivationDepth = activationDepth;
                    UpdaterIndex = 0;
                    SectionIndex = 0;
                }
            }
        }
    }
}
