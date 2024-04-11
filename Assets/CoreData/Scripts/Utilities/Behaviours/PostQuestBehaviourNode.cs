using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class PostQuestBehaviourNode : IRogueBehaviourNode
    {
        private PostboxInfo postboxInfo;
        private RogueObj postbox;
        private bool lobby;

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            if (postboxInfo == null)
            {
                postboxInfo = GetLobbyPostboxInfo(self, out postbox);
                if (postboxInfo == null) return RogueObjUpdaterContinueType.Continue;
            }

            if (postbox.Location == self.Location)
            {
                lobby = true;
                return RogueObjUpdaterContinueType.Continue;
            }
            if (!lobby || !DungeonInfo.TryGet(self.Location, out _))
            {
                lobby = false;
                return RogueObjUpdaterContinueType.Continue;
            }

            // ロビーからダンジョンに移動したとき投稿
            var post = new RoguePost();
            post.Name = $"{self.Location.Main.InfoSet.Name}へ出発";
            post.From = self;
            Debug.Log(System.DateTime.UtcNow);
            Debug.Log(RogueDateTime.UtcNow());
            post.DateTime = RogueDateTime.UtcNow().ToString();
            postboxInfo.AddPost(post);
            lobby = false;

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
