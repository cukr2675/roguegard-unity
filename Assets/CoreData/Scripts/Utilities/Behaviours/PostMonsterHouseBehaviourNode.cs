using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class PostMonsterHouseBehaviourNode : IRogueBehaviourNode
    {
        private PostboxInfo postboxInfo;
        private RogueObj postbox;
        private RogueObj postLocation;
        private RoguePost post;
        private RecordingMessageWorkListener recordingListener;

        [System.Obsolete] public static DungeonRecorder test;

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
                    test = recordingListener.Recorder;
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
            post = new RoguePost();
            post.Name = "モンスターハウスに遭遇";
            post.From = self;
            post.DateTime = RogueDateTime.UtcNow().ToString();
            post.LiveState = RoguePostLiveState.Live;
            postboxInfo.AddPost(post);

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
