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
            return obj.TryGet<MemberInfo>(out _);
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
                var memberInfo = new MemberInfo();
                memberInfo.info = new LobbyMemberInfo();
                obj.SetInfo(memberInfo);
                info.lobbyMembers.Add(obj);
            }
        }

        public static bool Remove(RogueObj obj, RogueObj world = null)
        {
            var result = obj.RemoveInfo(typeof(MemberInfo));

            world ??= RogueWorld.GetWorld(obj);
            if (result && world.TryGet<WorldInfo>(out var info))
            {
                info.lobbyMembers.Remove(obj);
            }
            return result;
        }

        public static LobbyMemberInfo GetMemberInfo(RogueObj obj)
        {
            if (obj != null && obj.TryGet<MemberInfo>(out var info))
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
        private class MemberInfo : IRogueObjInfo
        {
            public LobbyMemberInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
