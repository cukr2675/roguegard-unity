using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class DeviceMessageWorkListener : IMessageWorkListener
    {
        public bool CanHandle(RogueObj location, Vector2Int position)
        {
            var subject = RogueDevice.Primary.Subject;
            if (!ViewInfo.TryGet(subject, out var view))
            {
                return subject.Location == location;
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
            view.AddView(subject);
            view.GetTile(position, out visible, out _, out _, out _);
            return visible;
        }

        public void Handle(IKeyword keyword, int integer)
        {
            RogueDevice.Add(keyword, integer);
        }

        public void Handle(IKeyword keyword, float number)
        {
            RogueDevice.Add(keyword, number);
        }

        public void Handle(IKeyword keyword, object other)
        {
            RogueDevice.Add(keyword, other);
        }

        public void HandleWork(IKeyword keyword, in RogueCharacterWork work)
        {
            RogueDevice.AddWork(keyword, work);
        }
    }
}
