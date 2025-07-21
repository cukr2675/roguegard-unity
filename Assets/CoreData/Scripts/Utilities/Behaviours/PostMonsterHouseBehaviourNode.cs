using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class PostMonsterHouseBehaviourNode : IRogueBehaviourNode
    {
        private PostboxInfo postboxInfo;
        private RogueObj postbox;
        private RogueObj postLocation;
        private readonly RoguePost post;
        private RecordingMessageWorkListener recordingListener;

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;
            if (!ViewInfo.TryGet(self, out var view)) return RogueObjUpdaterContinueType.Continue;

            if (postboxInfo == null)
            {
                postboxInfo = GetLobbyPostboxInfo(self, out postbox);
                if (postboxInfo == null) return RogueObjUpdaterContinueType.Continue;
            }

            // 階層を移動したら配信終了
            if (postLocation != null && postLocation != self.Location)
            {
                if (post != null)
                {
                    MessageWorkListener.RemoveListener(recordingListener);
                    post.LiveState = RoguePostLiveState.Done;
                }
                postLocation = null;
            }

            // 同じ階層では一回しか投稿しない
            if (postLocation != null) return RogueObjUpdaterContinueType.Continue;

            // クエスト中のみ録画する
            if (!DungeonQuestInfo.TryGetQuest(self, out var quest)) return RogueObjUpdaterContinueType.Continue;

            var enemyCount = 0;
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || !StatsEffectedValues.AreVS(obj, self)) continue;

                enemyCount++;
            }
            if (enemyCount < 10) return RogueObjUpdaterContinueType.Continue;

            // 見えてる敵の数が 10 以上になったら配信開始
            postboxInfo.AddPost(new RoguePost
            {
                Name = "モンスターハウスに遭遇",
                From = self,
                DateTime = RogueDateTime.UtcNow().ToString(),
                LiveState = RoguePostLiveState.Live
            });

            // 録画開始
            var recorder = new DungeonRecorder(quest, self.Location.Main.Stats.Lv);
            recordingListener = new RecordingMessageWorkListener(self, recorder);
            MessageWorkListener.AddListener(recordingListener);

            // 同じ階層では一回しか投稿しない
            postLocation = self.Location;

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
