using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Extensions
{
    public static class RoguePartyUtility
    {
        /// <summary>
        /// �^�[�Q�b�g�I�u�W�F�N�g�ׂ̗ֈړ�����B
        /// </summary>
        public static bool LocateNextToObj(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, float activationDepth, RogueObj target)
        {
            var targetLocation = target.Location;
            for (int i = 0; i < 8; i++)
            {
                var direction = new RogueDirection(i);
                var position = target.Position + direction.Forward;
                if (method.Locate(self, user, targetLocation, position, activationDepth)) return true;
            }
            return false;
        }

        /// <summary>
        /// �����ꂩ�̃p�[�e�B�����o�[�ׂ̗ֈړ�����B���������[�_�[�Ɠ�����ԂɌ��肳���B
        /// </summary>
        public static bool LocateNextToAnyMember(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, float activationDepth, RogueParty party)
        {
            var partyMembers = party.Members;
            var targetLocation = partyMembers[0].Location;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member == null || member.Location != targetLocation) continue;

                if (method.LocateNextToObj(self, user, activationDepth, member)) return true;
            }
            return false;
        }

        /// <summary>
        /// �����ꂩ�̃p�[�e�B�����o�[�ׂ̗ֈړ�����B���������[�_�[�Ɠ�����ԂɌ��肳���B
        /// </summary>
        public static bool TryLocateNextToAnyMember(RogueObj self, RogueParty party)
        {
            var partyMembers = party.Members;
            var targetLocation = partyMembers[0].Location;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member == null || member.Location != targetLocation) continue;

                for (int j = 0; j < 8; j++)
                {
                    var direction = new RogueDirection(j);
                    var position = member.Position + direction.Forward;
                    if (SpaceUtility.TryLocate(self, member.Location, position)) return true;
                }
            }
            return false;
        }

        public static bool LocateWithPartyMembers(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, RogueObj location, float activationDepth, bool resetStack)
        {
            if (self.Stack == 0 && resetStack) { self.TrySetStack(1); }
            var result = method.Locate(self, user, location, activationDepth);
            var party = self.Main.Stats.Party;
            if (result && party != null)
            {
                var partyMembers = party.Members;
                for (int i = 0; i < partyMembers.Count; i++)
                {
                    var member = partyMembers[i];
                    if (member == self) continue;
                    if (member.Stack == 0)
                    {
                        if (resetStack) { member.TrySetStack(1); }
                        else continue;
                    }
                    if (!method.Locate(member, user, location, activationDepth) && !SpaceUtility.TryLocate(member, location))
                    {
                        Debug.LogError($"{member} �̈ړ��Ɏ��s���܂����B");
                    }
                }
            }
            return result;
        }

        public static bool TryLocateWithPartyMembers(RogueObj self, RogueObj location, bool resetStack)
        {
            if (self.Stack == 0 && resetStack) { self.TrySetStack(1); }
            var result = SpaceUtility.TryLocate(self, location);
            var party = self.Main.Stats.Party;
            if (result && party != null)
            {
                var partyMembers = party.Members;
                for (int i = 0; i < partyMembers.Count; i++)
                {
                    var member = partyMembers[i];
                    if (member == self) continue;
                    if (member.Stack == 0)
                    {
                        if (resetStack) { member.TrySetStack(1); }
                        else continue;
                    }
                    if (!SpaceUtility.TryLocate(member, location))
                    {
                        Debug.LogError($"{member} �̈ړ��Ɏ��s���܂����B");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// �_���W�����ɓ˓��E�E�o�����Ƃ��̏����B�G�t�F�N�g�̉����E���x���������E�S�񕜂������Ȃ��B
        /// </summary>
        public static void Reset(RogueParty party, IRogueEffect leaderEffect)
        {
            var partyMembers = party.Members;
            {
                // ���[�_�[�p�G�t�F�N�g��ݒ�
                var leader = partyMembers[0];
                leader.Main.RogueEffects.AddOpen(leader, leaderEffect);
            }
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (i != 0)
                {
                    // ���[�_�[�ȊO�̓G�t�F�N�g������
                    member.Main.UpdatePlayerLeaderInfo(member, null);
                }

                DungeonFloorCloserStateInfo.CloseAndRemoveNull(member, true);

                // ���x���K���X�L����Y�ꂳ����
                member.Main.Skills.Clear(MainInfoSetType.Base);
                member.Main.Skills.Clear(MainInfoSetType.Polymorph);

                // ���x����������
                RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(member, 0);
                RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(member, 1);

                // �T���J�n�O�ɑS�񕜂���
                member.Main.Stats.Reset(member);

                member.Main.Stats.Direction = RogueDirection.Down;
            }
        }

        public static void CloseDungeonFloorClosers(RogueParty party, bool exitDungeon)
        {
            var partyMembers = party.Members;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                DungeonFloorCloserStateInfo.CloseAndRemoveNull(member, exitDungeon);
            }
        }

        public static void AssignWithPartyMembers(RogueObj self, RogueParty to)
        {
            var from = self.Main.Stats.Party;
            if (from != null)
            {
                while (from.Members.Count >= 1)
                {
                    var member = from.Members[0];
                    member.Main.Stats.TryAssignParty(member, to);
                }
            }
            else
            {
                self.Main.Stats.TryAssignParty(self, to);
            }
        }
    }
}
