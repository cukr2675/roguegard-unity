using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AcceptQuestBehaviourNode : IRogueBehaviourNode
    {
        private int count;

        private const int minCount = 10;
        private const int maxCount = 20;

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            var nearestQuestBoardInfo = GetNearestQuestBoardInfo(self);

            // �N�G�X�g���󒍍ς݂Ȃ�ҋ@����
            if (nearestQuestBoardInfo.QuestTable.Contains(self.Main.Stats.Party)) return RogueObjUpdaterContinueType.Break;

            if (count <= 0)
            {
                // �c��J�E���g���ݒ肳��Ă��Ȃ���΁A�����ŃJ�E���g��ݒ肷��
                count = RogueRandom.Primary.Next(minCount, maxCount);

                // �����Ɍ��݂̃p�[�e�B���甲����
                // ���������[�_�[�̂Ƃ��̓p�[�e�B����̂���
                var party = self.Main.Stats.Party;
                if (party != null)
                {
                    if (party.Members[0] == self)
                    {
                        while (party.Members.Count >= 1)
                        {
                            var member = party.Members[0];
                            var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                            member.Main.Stats.TryAssignParty(member, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                        }
                    }
                    else
                    {
                        var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                        self.Main.Stats.TryAssignParty(self, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                    }
                }
            }

            // �J�E���g���[���ɂȂ�O�ɁA�����N�G�X�g�{�[�h����󒍍ς݂̃��r�[�����o�[�������炻���ɎQ������
            if (nearestQuestBoardInfo.TryAutoAssign(self))
            {
                count = 0;
                return RogueObjUpdaterContinueType.Break;
            }

            // �J�E���g���[���ɂȂ����Ƃ��N�G�X�g����
            count--;
            if (count <= 0)
            {
                count = 0;
                nearestQuestBoardInfo.QuestTable.TryAcceptAt(0, self.Main.Stats.Party, nearestQuestBoardInfo.WeightTurnsAfterAccept);
                return RogueObjUpdaterContinueType.Break;
            }

            return RogueObjUpdaterContinueType.Continue;
        }

        private static QuestBoardInfo GetNearestQuestBoardInfo(RogueObj self)
        {
            var locationObjs = self.Location.Space.Objs;
            QuestBoardInfo nearestQuestBoardInfo = null;
            var nearestSqrDistance = int.MaxValue;
            for (int i = 0; i < locationObjs.Count; i++)
            {
                var obj = locationObjs[i];
                if (obj == null) continue;

                var info = QuestBoardInfo.Get(obj);
                if (info == null) continue;

                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestQuestBoardInfo = info;
                    nearestSqrDistance = sqrDistance;
                }
            }
            return nearestQuestBoardInfo;
        }
    }
}
