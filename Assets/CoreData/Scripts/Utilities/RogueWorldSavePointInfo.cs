using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueWorldSavePointInfo : ISavePointInfo
    {
        public static RogueWorldSavePointInfo Instance { get; } = new RogueWorldSavePointInfo();

        public IApplyRogueMethod BeforeSave => _beforeSave;
        private static readonly IApplyRogueMethod _beforeSave = new BeforeSaveRogueMethod();

        public IApplyRogueMethod AfterLoad => _afterLoad;
        private static readonly IApplyRogueMethod _afterLoad = new AfterLoadRogueMethod();

        private static readonly LobbyLeaderEffect lobbyLeaderEffect = new LobbyLeaderEffect();

        private class BeforeSaveRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
            {
                if (LobbyMemberList.GetMemberInfo(player) == null) return false;

                var world = RogueWorldInfo.GetWorld(RogueDevice.Primary.Player);
                var dungeon = DungeonInfo.GetLargestDungeon(player);

                if (player.Location != world)
                {
                    // ���[���h�ɂ��Ȃ��ꍇ

                    // �v���C���[�L�����N�^�[�ƃp�[�e�B�����o�[�͕ʋ�ԂɈړ�������B
                    var result = this.LocateWithPartyMembers(player, null, world, activationDepth, true);
                    if (!result) return false;

                    // �������_���W�����͏����B
                    if (dungeon != null)
                    {
                        dungeon.TrySetStack(0);
                        world.Space.RemoveAllNull();
                    }
                }

                // �p�[�e�B�E���[�_�[�G�t�F�N�g�E���x���A�b�v�{�[�i�X�̏�����
                var party = new RogueParty(player.Main.InfoSet.Faction, player.Main.InfoSet.TargetFactions);
                RoguePartyUtility.AssignWithPartyMembers(player, party);

                RoguePartyUtility.Reset(party, lobbyLeaderEffect);

                if (player == RogueDevice.Primary.Player)
                {
                    // �v���C���[�L�����Ȃ�ŏ��͏����
                    player.Main.Stats.Direction = RogueDirection.Up;
                }

                return true;
            }
        }

        private class AfterLoadRogueMethod : FloorMenuAfterLoadRogueMethod
        {
            protected override string GetName(RogueMenuManager manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                return "���r�[";
            }

            protected override void Activate(RogueMenuManager manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var world = RogueWorldInfo.GetWorld(player);
                var worldInfo = RogueWorldInfo.GetByCharacter(player);
                var tilemap = worldInfo.Lobby.Space.Tilemap;
                var memberInfo = LobbyMemberList.GetMemberInfo(player);
                if (memberInfo.Seat != null && memberInfo.Seat.Location == null)
                {
                    // �Ȃ������Ă����疳����
                    memberInfo.Seat = null;
                }
                Vector2Int position;
                if (memberInfo.Seat != null && memberInfo.Seat.Location == worldInfo.Lobby)
                {
                    position = memberInfo.Seat.Position;
                }
                else
                {
                    // ���X�|�[���n�_���ݒ肳��Ă��Ȃ���΋K��̈ʒu���g�p
                    position = new Vector2Int(tilemap.Width / 2, 3);
                }
                if (!SpaceUtility.TryLocate(player, worldInfo.Lobby, position))
                {
                    // �ړ��Ɏ��s������ǒʉߏ�Ԃňړ�������
                    var movement = MovementCalculator.Get(player);
                    if (!player.TryLocate(worldInfo.Lobby, position, movement.AsTile, false, false, movement.HasSightCollider, StackOption.Default))
                        throw new RogueException("�Z�[�u�|�C���g����̕��A�Ɏ��s���܂����B���A�ʒu�Ɉړ��ł��܂���B");
                }
                // �p�[�e�B�����o�[���ړ�
                if (player.Main.Stats.Party != null)
                {
                    var partyMembers = player.Main.Stats.Party.Members;
                    for (int i = 0; i < partyMembers.Count; i++)
                    {
                        var member = partyMembers[i];
                        if (member == player) continue;
                        var memberMemberInfo = LobbyMemberList.GetMemberInfo(member);
                        if (memberMemberInfo.Seat.Location == null)
                        {
                            // �Ȃ������Ă����疳����
                            memberMemberInfo.Seat = null;
                        }
                        if (memberMemberInfo.Seat != null && memberMemberInfo.Seat.Location == worldInfo.Lobby)
                        {
                            var memberPosition = memberMemberInfo.Seat.Position;
                            if (SpaceUtility.TryLocate(member, worldInfo.Lobby, memberPosition)) continue;
                        }

                        // ���X�|�[���n�_���ݒ肳��Ă��Ȃ���΋K��̈ʒu���g�p
                        if (!RoguePartyUtility.TryLocateNextToAnyMember(member, player.Main.Stats.Party))
                        {
                            Debug.LogError($"{member} �̈ړ��Ɏ��s���܂����B");
                        }
                    }
                }

                world.Space.RemoveAllNull();
            }
        }

        /// <summary>
        /// ���r�[�̃v���C���[�p�[�e�B�����o�[�͎��R�񕜂���B
        /// </summary>
        [Objforming.Formable]
        private class LobbyLeaderEffect : PlayerLeaderRogueEffect, IValueEffect, IRogueObjUpdater
        {
            float IValueEffect.Order => 0f;
            float IRogueObjUpdater.Order => 100f;

            private static readonly MemberEffect memberEffect = new MemberEffect();

            void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.LoadCapacity)
                {
                    value.MainValue = Mathf.Infinity;
                }
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                // ���[�_�[�̎��R��
                self.Main.Stats.Regenerate(self);

                // �p�[�e�B�����o�[�Ɏ��R�񕜌��ʂ�t�^
                memberEffect.AffectToPartyMembersOf(self, false);
                return default;
            }

            public override bool Equals(object obj) => obj.GetType() == GetType();
            public override int GetHashCode() => GetType().GetHashCode();

            /// <summary>
            /// ���̃G�t�F�N�g���t�^����Ă���I�u�W�F�N�g�͎��R�񕜂�L���ɂ���B
            /// </summary>
            [Objforming.Formable]
            private class MemberEffect : BasePartyMemberRogueEffect<LobbyLeaderEffect>
            {
                protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
                {
                    self.Main.Stats.Regenerate(self);
                    return default;
                }
            }
        }
    }
}
