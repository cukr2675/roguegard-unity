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
            postboxInfo.AddPost(new RoguePost
            {
                Name = $"{self.Location.Main.InfoSet.Name}へ出発",
                From = self,
                DateTime = RogueDateTime.UtcNow().ToString()
            });
            lobby = false;

            return RogueObjUpdaterContinueType.Continue;
        }

        private static PostboxInfo GetLobbyPostboxInfo(RogueObj self, out RogueObj postbox)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(self);
            foreach (var obj in worldInfo.Lobby.Space.Objs)
            {
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
