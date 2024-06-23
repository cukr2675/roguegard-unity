using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueMethodAspectState
    {
        private readonly List<ActiveItem> actives;
        private readonly List<PassiveItem> passives;

        private int nextID;

        private static readonly StaticInitializable<AspectStack> stack = new StaticInitializable<AspectStack>(() => new AspectStack());

        public static bool ActivatingNow => stack.Value.ContainsItem;

        public static IRogueMethodAspectLogger Logger { get; set; }

        public RogueMethodAspectState()
        {
            actives = new List<ActiveItem>();
            passives = new List<PassiveItem>();
            nextID = 0;
        }

        public void AddActiveFromInfoSet(RogueObj self, IRogueMethodActiveAspect aspect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (aspect == null) throw new System.ArgumentNullException(nameof(aspect));

            var newItem = new ActiveItem(aspect, nextID);
            nextID++;
            for (int i = 0; i < actives.Count; i++)
            {
                // 同じ Order の要素が存在するときその手前に追加する。
                if (actives[i].Aspect != null && actives[i].Aspect.Order >= aspect.Order)
                {
                    actives.Insert(i, newItem);
                    return;
                }
            }
            actives.Add(newItem);
        }

        public void AddActiveFromRogueEffect(RogueObj self, IRogueMethodActiveAspect aspect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (aspect == null) throw new System.ArgumentNullException(nameof(aspect));

            var newItem = new ActiveItem(aspect, nextID);
            nextID++;
            for (int i = actives.Count - 1; i >= 0; i--)
            {
                // 同じ Order の要素が存在するときその後ろに追加する。
                if (actives[i].Aspect != null && actives[i].Aspect.Order <= aspect.Order)
                {
                    actives.Insert(i + 1, newItem);
                    return;
                }
            }
            actives.Insert(0, newItem);
        }

        public bool ReplaceActiveWithNull(IRogueMethodActiveAspect aspect)
        {
            var index = IndexOf();
            if (index < 0) return false;

            var item = actives[index];
            item.Aspect = default;
            actives[index] = item;
            return true;

            int IndexOf()
            {
                for (int i = 0; i < actives.Count; i++)
                {
                    if (actives[i].Aspect == aspect) return i;
                }
                return -1;
            }
        }

        public void AddPassiveFromInfoSet(RogueObj self, IRogueMethodPassiveAspect aspect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (aspect == null) throw new System.ArgumentNullException(nameof(aspect));

            var newItem = new PassiveItem(aspect, nextID);
            nextID++;
            for (int i = 0; i < passives.Count; i++)
            {
                // 同じ Order の要素が存在するときその手前に追加する。
                if (passives[i].Aspect != null && passives[i].Aspect.Order >= aspect.Order)
                {
                    passives.Insert(i, newItem);
                    return;
                }
            }
            passives.Add(newItem);
        }

        public void AddPassiveFromRogueEffect(RogueObj self, IRogueMethodPassiveAspect aspect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (aspect == null) throw new System.ArgumentNullException(nameof(aspect));

            var newItem = new PassiveItem(aspect, nextID);
            nextID++;
            for (int i = passives.Count - 1; i >= 0; i--)
            {
                // 同じ Order の要素が存在するときその後ろに追加する。
                if (passives[i].Aspect != null && passives[i].Aspect.Order <= aspect.Order)
                {
                    passives.Insert(i + 1, newItem);
                    return;
                }
            }
            passives.Insert(0, newItem);
        }

        public bool ReplacePassiveWithNull(IRogueMethodPassiveAspect aspect)
        {
            var index = IndexOf();
            if (index < 0) return false;

            var item = passives[index];
            item.Aspect = default;
            passives[index] = item;
            return true;

            int IndexOf()
            {
                for (int i = 0; i < passives.Count; i++)
                {
                    if (passives[i].Aspect == aspect) return i;
                }
                return -1;
            }
        }

        public void RemoveAllNull()
        {
            if (stack.Value.ContainsItem) throw new RogueException();

            for (int i = actives.Count - 1; i >= 0; i--)
            {
                if (actives[i].Aspect == null) actives.RemoveAt(i);
            }
            for (int i = passives.Count - 1; i >= 0; i--)
            {
                if (passives[i].Aspect == null) passives.RemoveAt(i);
            }
        }

        public static bool Invoke(
            IKeyword keyword, IActiveRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return Invoke(0, keyword, method, self, user, activationDepth, arg);
        }

        public static bool Invoke(
            IKeyword keyword, IApplyRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return Invoke(1, keyword, method, self, user, activationDepth, arg);
        }

        public static bool Invoke(
            IKeyword keyword, IAffectRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return Invoke(2, keyword, method, self, user, activationDepth, arg);
        }

        public static bool Invoke(
            IKeyword keyword, IChangeStateRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return Invoke(3, keyword, method, self, user, activationDepth, arg);
        }

        public static bool Invoke(
            IKeyword keyword, IChangeEffectRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return Invoke(4, keyword, method, self, user, activationDepth, arg);
        }

        private static bool Invoke(
            int rank, IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (keyword == null) throw new System.ArgumentNullException(nameof(keyword));
            if (method == null) throw new System.ArgumentNullException(nameof(method), $"{self} の {keyword.Name} メソッドが Null です。");

            Logger?.LogInvoke(AspectStack.GetRankName(rank), keyword, method, self, user, activationDepth, arg);

            stack.Value.Push(rank, activationDepth);
            bool result;
            if (user != null)
            {
                var aspectState = user.Main.GetRogueMethodAspectState(user);
                result = aspectState.ActiveInvoke(keyword, method, user, self, activationDepth, arg);
            }
            else if (self != null)
            {
                stack.Value.GetPeek(out var oldIndex, out var id);
                stack.Value.SetPeek(0, -1);
                var targetAspectState = self.Main.GetRogueMethodAspectState(self);
                result = targetAspectState.PassiveInvoke(keyword, method, self, user, activationDepth, arg);
                stack.Value.SetPeek(oldIndex, id);
            }
            else
            {
                result = method.Invoke(self, user, activationDepth, arg);
            }

            Logger?.LogEndInvoke(AspectStack.GetRankName(rank), keyword, method, self, user, activationDepth, arg, result);
            stack.Value.Pop();
            return result;
        }

        private bool ActiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg)
        {
            stack.Value.GetPeek(out var oldIndex, out var id);
            var index = oldIndex;
            var item = GetItem(ref index);
            if (index >= actives.Count)
            {
                if (target == null) return method.Invoke(target, self, activationDepth, arg);

                stack.Value.SetPeek(0, -1);
                var targetAspectState = target.Main.GetRogueMethodAspectState(target);
                var passiveResult = targetAspectState.PassiveInvoke(keyword, method, target, self, activationDepth, arg);
                stack.Value.SetPeek(oldIndex, id);
                return passiveResult;
            }

            Logger?.LogActiveAspect(item.Aspect, keyword, method, self, target, activationDepth, arg);

            stack.Value.SetPeek(index, actives[index].ID);
            var chain = new ActiveChain(this);
            var result = item.Aspect.ActiveInvoke(keyword, method, self, target, activationDepth, arg, chain);

            stack.Value.SetPeek(oldIndex, id);
            return result;

            ActiveItem GetItem(ref int index)
            {
                for (; index < actives.Count; index++)
                {
                    var item = actives[index];
                    if (item.ID == id)
                    {
                        // ID が一致した場合は null でない要素まで探索する。
                        index++;
                        for (; index < actives.Count; index++)
                        {
                            item = actives[index];
                            if (item.Aspect != null) return item;
                        }
                        return default;
                    }
                    if (id == -1)
                    {
                        // null でない要素まで探索する。
                        for (; index < actives.Count; index++)
                        {
                            item = actives[index];
                            if (item.Aspect != null) return item;
                        }
                        return default;
                    }
                }
                return default;
            }
        }

        private bool PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            stack.Value.GetPeek(out var oldIndex, out var id);
            var index = oldIndex;
            var item = GetItem(ref index);
            if (index >= passives.Count) return method.Invoke(self, user, activationDepth, arg);

            Logger?.LogPassiveAspect(item.Aspect, keyword, method, self, user, activationDepth, arg);

            stack.Value.SetPeek(index, passives[index].ID);
            var chain = new PassiveChain(this);
            var result = item.Aspect.PassiveInvoke(keyword, method, self, user, activationDepth, arg, chain);

            stack.Value.SetPeek(oldIndex, id);
            return result;

            PassiveItem GetItem(ref int index)
            {
                for (; index < passives.Count; index++)
                {
                    var item = passives[index];
                    if (item.ID == id)
                    {
                        // ID が一致した場合は null でない要素まで探索する。
                        index++;
                        for (; index < passives.Count; index++)
                        {
                            item = passives[index];
                            if (item.Aspect != null) return item;
                        }
                        return default;
                    }
                    if (id == -1)
                    {
                        // null でない要素まで探索する。
                        for (; index < passives.Count; index++)
                        {
                            item = passives[index];
                            if (item.Aspect != null) return item;
                        }
                        return default;
                    }
                }
                return default;
            }
        }

        private struct ActiveItem
        {
            public IRogueMethodActiveAspect Aspect;
            public readonly int ID;
            public ActiveItem(IRogueMethodActiveAspect activeAspect, int id)
            {
                Aspect = activeAspect;
                ID = id;
            }
        }

        private struct PassiveItem
        {
            public IRogueMethodPassiveAspect Aspect;
            public readonly int ID;
            public PassiveItem(IRogueMethodPassiveAspect passiveAspect, int id)
            {
                Aspect = passiveAspect;
                ID = id;
            }
        }

        public ref struct ActiveChain
        {
            private readonly RogueMethodAspectState state;
            internal ActiveChain(RogueMethodAspectState state) { this.state = state; }
            public bool Invoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg)
                => state.ActiveInvoke(keyword, method, self, target, activationDepth, arg);
        }

        public ref struct PassiveChain
        {
            private readonly RogueMethodAspectState state;
            internal PassiveChain(RogueMethodAspectState state) { this.state = state; }
            public bool Invoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
                => state.PassiveInvoke(keyword, method, self, user, activationDepth, arg);
        }
    }
}
