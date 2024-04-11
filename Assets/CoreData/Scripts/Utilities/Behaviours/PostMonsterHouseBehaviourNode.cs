using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class PostMonsterHouseBehaviourNode : IRogueBehaviourNode
    {
        private PostboxInfo postboxInfo;
        private RogueObj postbox;
        private RogueObj postLocation;
        private RoguePost post;

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;
            if (!ViewInfo.TryGet(self, out var view)) return RogueObjUpdaterContinueType.Continue;

            if (postboxInfo == null)
            {
                postboxInfo = GetLobbyPostboxInfo(self, out postbox);
                if (postboxInfo == null) return RogueObjUpdaterContinueType.Continue;
            }

            // äKëwÇà⁄ìÆÇµÇΩÇÁîzêMèIóπ
            if (postLocation != self.Location)
            {
                if (post != null) { post.LiveState = RoguePostLiveState.Done; }
                postLocation = null;
            }

            // ìØÇ∂äKëwÇ≈ÇÕàÍâÒÇµÇ©ìäçeÇµÇ»Ç¢
            if (postLocation != null) return RogueObjUpdaterContinueType.Continue;

            var enemyCount = 0;
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || !StatsEffectedValues.AreVS(obj, self)) continue;

                enemyCount++;
            }
            if (enemyCount < 10) return RogueObjUpdaterContinueType.Continue;

            // å©Ç¶ÇƒÇÈìGÇÃêîÇ™ 10 à»è„Ç…Ç»Ç¡ÇΩÇÁîzêMäJén
            post = new RoguePost();
            post.Name = "ÉÇÉìÉXÉ^Å[ÉnÉEÉXÇ…ëòãˆ";
            post.From = self;
            post.DateTime = RogueDateTime.UtcNow().ToString();
            post.LiveState = RoguePostLiveState.Live;
            postboxInfo.AddPost(post);

            // ìØÇ∂äKëwÇ≈ÇÕàÍâÒÇµÇ©ìäçeÇµÇ»Ç¢
            postLocation = self.Location;

            return RogueObjUpdaterContinueType.Continue;
        }

        private static PostboxInfo GetLobbyPostboxInfo(RogueObj self, out RogueObj postbox)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(self);
            var lobbyObjs = worldInfo.Lobby.Space.Objs;
            for (int i = 0; i < lobbyObjs.Count; i++)
            {
                var obj = lobbyObjs[i];
                if (obj == null) continue;

                var info = PostboxInfo.Get(obj);
                if (info == null) continue;

                postbox = obj;
                return info;
            }
            postbox = null;
            return null;
        }
    }
}
