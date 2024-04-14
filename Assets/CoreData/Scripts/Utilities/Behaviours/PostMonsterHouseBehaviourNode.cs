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

            // �K�w���ړ�������z�M�I��
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

            // �����K�w�ł͈�񂵂����e���Ȃ�
            if (postLocation != null) return RogueObjUpdaterContinueType.Continue;

            // �N�G�X�g���̂ݘ^�悷��
            if (!DungeonQuestInfo.TryGetQuest(self, out var quest)) return RogueObjUpdaterContinueType.Continue;

            var enemyCount = 0;
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || !StatsEffectedValues.AreVS(obj, self)) continue;

                enemyCount++;
            }
            if (enemyCount < 10) return RogueObjUpdaterContinueType.Continue;

            // �����Ă�G�̐��� 10 �ȏ�ɂȂ�����z�M�J�n
            post = new RoguePost();
            post.Name = "�����X�^�[�n�E�X�ɑ���";
            post.From = self;
            post.DateTime = RogueDateTime.UtcNow().ToString();
            post.LiveState = RoguePostLiveState.Live;
            postboxInfo.AddPost(post);

            // �^��J�n
            var recorder = new DungeonRecorder(quest, self.Location.Main.Stats.Lv);
            recordingListener = new RecordingMessageWorkListener(self, recorder);
            MessageWorkListener.AddListener(recordingListener);

            // �����K�w�ł͈�񂵂����e���Ȃ�
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
