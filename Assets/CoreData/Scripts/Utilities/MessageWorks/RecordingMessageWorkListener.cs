using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RecordingMessageWorkListener : IMessageWorkListener
    {
        private readonly RogueObj owner;
        public DungeonRecorder Recorder { get; }

        public RecordingMessageWorkListener(RogueObj owner, DungeonRecorder recorder)
        {
            this.owner = owner;
            Recorder = recorder;
        }

        public bool CanHandle(RogueObj location, Vector2Int position)
        {
            if (!ViewInfo.TryGet(owner, out var view))
            {
                return owner.Location == location;
            }
            if (view.Location != location || view.Location.Space.Tilemap == null) return false;

            view.GetTile(position, out var visible, out _, out _, out _);
            if (visible) return true;

            // �y�C���g����Ă���I�u�W�F�N�g������ʒu�͌�����
            var obj = view.Location.Space.GetColliderObj(position);
            if (obj != null)
            {
                var objStatusEffectState = obj.Main.GetStatusEffectState(obj);
                if (objStatusEffectState.TryGetStatusEffect<PaintStatusEffect>(out _)) return true;
            }

            // ���E�͈͊O�̔��肪�o�Ă��A�X�V���Ă�����x����
            // �o�������̓G��\������ۂɗL��
            view.AddView(owner);
            view.GetTile(position, out visible, out _, out _, out _);
            return visible;
        }

        public void Handle(IKeyword keyword, int integer)
        {
            Recorder.Add(keyword, integer);
        }

        public void Handle(IKeyword keyword, float number)
        {
            Recorder.Add(keyword, number);
        }

        public void Handle(IKeyword keyword, object other)
        {
            Recorder.Add(keyword, other);
        }

        public void HandleWork(IKeyword keyword, in RogueCharacterWork work)
        {
            Recorder.AddWork(keyword, work);
        }
    }
}
