using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class LobbyMemberList
    {
        private readonly List<RogueObj> _members = new List<RogueObj>();

        public Spanning<RogueObj> Members => _members;

        public void Add(RogueObj character)
        {
            if (_members.Contains(character)) return;

            var memberEffect = new MemberEffect();
            memberEffect.info = new LobbyMemberInfo();
            character.Main.RogueEffects.AddOpen(character, memberEffect);
            _members.Add(character);
        }

        public bool Remove(RogueObj character)
        {
            if (character.Main.RogueEffects.TryGetEffect<MemberEffect>(out var memberEffect))
            {
                RogueEffectUtility.RemoveClose(character, memberEffect);
                character.RemoveInfo(typeof(MemberEffect));
                _members.Remove(character);
                return true;
            }
            return false;
        }

        public static LobbyMemberInfo GetMemberInfo(RogueObj character)
        {
            character.Main.TryOpenRogueEffects(character);

            if (character != null && character.TryGet<MemberEffect>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        [ObjectFormer.Formable]
        private class MemberEffect : IRogueEffect, IRogueObjUpdater, IRogueObjInfo
        {
            public LobbyMemberInfo info;

            [System.NonSerialized] private bool leader;

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
                var currentLeader = (info.Seat != null) && (self.Main.Stats.Party.Members[0] == self);
                if (leader == currentLeader && priority <= effectedPriority) return default;

                RogueBehaviourNodeEffect.RemoveBehaviourNode(self, priority);
                leader = currentLeader;
                if (leader)
                {
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
                }
                else
                {
                    var node = new RogueBehaviourNodeList();
                    node.Add(new AttackBehaviourNode());
                    var follow = new FollowLeaderBehaviourNode();
                    follow.PathBuilder = new AStarPathBuilder(RoguegardSettings.MaxTilemapSize);
                    node.Add(follow);
                    RogueBehaviourNodeEffect.SetBehaviourNode(self, node, priority);
                }
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
