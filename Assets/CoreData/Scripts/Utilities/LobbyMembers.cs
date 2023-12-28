using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

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
                obj.SetInfo(new MemberInfo());
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

        public static CharacterCreationDataBuilder GetCharacterCreationDataBuilder(RogueObj obj)
        {
            if (obj.TryGet<MemberInfo>(out var info))
            {
                return info.Data;
            }
            else
            {
                return null;
            }
        }

        public static void SetCharacterCreationDataBuilder(RogueObj obj, CharacterCreationDataBuilder builder)
        {
            if (obj.TryGet<MemberInfo>(out var info))
            {
                info.Data = builder;
            }
            else
            {
                Debug.LogWarning($"{obj} はロビーメンバーではありません。");
            }
        }

        public static ISavePointInfo GetSavePoint(RogueObj obj)
        {
            if (obj.TryGet<MemberInfo>(out var info))
            {
                return info.SavePoint;
            }
            else
            {
                return null;
            }
        }

        public static void SetSavePoint(RogueObj obj, ISavePointInfo savePoint)
        {
            if (obj.TryGet<MemberInfo>(out var info))
            {
                info.SavePoint = savePoint;
            }
            else
            {
                Debug.LogWarning($"{obj} はロビーメンバーではありません。");
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
            public CharacterCreationDataBuilder Data { get; set; }

            public ISavePointInfo SavePoint { get; set; }

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
