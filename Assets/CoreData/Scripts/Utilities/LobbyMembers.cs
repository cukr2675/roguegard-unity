using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class LobbyMembers
    {
        public static Spanning<RogueObj> GetMembersByCharacter(RogueObj obj)
        {
            var world = RogueWorld.GetWorld(obj);
            if (world.TryGet<WorldInfo>(out var info))
            {
                return info.lobbyMembers;
            }
            else
            {
                return Spanning<RogueObj>.Empty;
            }
        }

        public static bool Contains(RogueObj obj)
        {
            return obj.TryGet<MemberEffect>(out _);
        }

        public static void Add(RogueObj obj, RogueObj world = null)
        {
            world ??= RogueWorld.GetWorld(obj);
            if (!world.TryGet<WorldInfo>(out var info))
            {
                info = new WorldInfo();
                world.SetInfo(info);
            }

            if (!Contains(obj))
            {
                var memberEffect = new MemberEffect();
                memberEffect.info = new LobbyMemberInfo();
                obj.Main.RogueEffects.AddOpen(obj, memberEffect);
                info.lobbyMembers.Add(obj);
            }
        }

        public static bool Remove(RogueObj obj, RogueObj world = null)
        {
            if (obj.Main.RogueEffects.TryGetEffect<MemberEffect>(out var memberEffect))
            {
                RogueEffectUtility.RemoveClose(obj, memberEffect);
                world ??= RogueWorld.GetWorld(obj);
                var info = world.Get<WorldInfo>();
                info.lobbyMembers.Remove(obj);
                return true;
            }
            return false;
        }

        public static LobbyMemberInfo GetMemberInfo(RogueObj obj)
        {
            obj.Main.TryOpenRogueEffects(obj);

            if (obj != null && obj.TryGet<MemberEffect>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        [ObjectFormer.Formable]
        private class WorldInfo : IRogueObjInfo
        {
            public RogueObjList lobbyMembers = new RogueObjList();

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }

        [ObjectFormer.Formable]
        private class MemberEffect : IRogueEffect, IRogueObjUpdater, IRogueObjInfo
        {
            public LobbyMemberInfo info;

            public bool IsExclusedWhenSerialize => true;

            float IRogueObjUpdater.Order => -priority;

            private const float priority = 2f;

            void IRogueEffect.Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
                self.SetInfo(this);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                var effectedPriority = RogueBehaviourNodeEffect.GetPriority(self);
                if (priority <= effectedPriority) return default;

                if (info.Seat == null)
                {
                    if (priority == effectedPriority) { RogueBehaviourNodeEffect.RemoveBehaviourNode(self, priority); }
                    return default;
                }

                if (!self.TryGet<ViewInfo>(out _))
                {
                    self.SetInfo(new ViewInfo());
                }

                var ifInLobby = new IfInLobbyBehaviourNode();
                ifInLobby.InLobbyNode.Add(new StorageBehaviourNode());
                ifInLobby.InLobbyNode.Add(new AcceptQuestBehaviourNode());
                ifInLobby.OtherNode.Add(new AttackBehaviourNode());
                ifInLobby.OtherNode.Add(new PushObstacleBehaviourNode());

                var pickUp = new PickUpBehaviourNode();
                pickUp.DistanceThreshold = 10;
                pickUp.PathBuilder = new AStarPathBuilder(RoguegardSettings.MaxTilemapSize);
                ifInLobby.OtherNode.Add(pickUp);

                var explore = new ExploreForStairsBehaviourNode();
                explore.PathBuilder = pickUp.PathBuilder;
                explore.PositionSelector = new RoguePositionSelector();
                ifInLobby.OtherNode.Add(explore);

                var node = new RogueBehaviourNodeList();
                node.Add(ifInLobby);
                RogueBehaviourNodeEffect.SetBehaviourNode(self, node, priority);
                return default;
            }

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;

            bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
            IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                info.ItemRegister.ReplaceCloned(obj, clonedObj);
                return this;
            }
        }
    }
}
