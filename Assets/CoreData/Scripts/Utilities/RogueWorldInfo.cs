using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueWorldInfo
    {
        public RogueObj Lobby { get; }

        public LobbyMemberList LobbyMembers { get; }

        private RogueWorldInfo() { }

        private RogueWorldInfo(RogueObj lobby)
        {
            Lobby = lobby;
            LobbyMembers = new LobbyMemberList();
        }

        public static void SetTo(RogueObj world, RogueObj lobby)
        {
            var info = new Info();
            info.info = new RogueWorldInfo(lobby);
            world.SetInfo(info);
        }

        public static RogueObj GetWorld(RogueObj self)
        {
            var location = self;
            while (true)
            {
                if (location == null) throw new RogueException("ワールドにあたるオブジェクトが存在しません。");
                if (location.TryGet<Info>(out _)) return location;

                location = location.Location;
            }
        }

        public static RogueWorldInfo GetByCharacter(RogueObj self)
        {
            var world = GetWorld(self);
            if (world.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public RogueWorldInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
