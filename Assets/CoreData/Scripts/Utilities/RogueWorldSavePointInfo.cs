using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RogueWorldSavePointInfo : ISavePointInfo
    {
        public static RogueWorldSavePointInfo Instance { get; } = new RogueWorldSavePointInfo();

        public IApplyRogueMethod BeforeSave => _beforeSave;
        private static readonly IApplyRogueMethod _beforeSave = new BeforeSaveRogueMethod();

        public IApplyRogueMethod AfterLoad => _afterLoad;
        private static readonly IApplyRogueMethod _afterLoad = new AfterLoadRogueMethod();

        private class BeforeSaveRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
            {
                if (LobbyMemberList.GetMemberInfo(player) == null) return false;

                var world = RogueWorldInfo.GetWorld(player);
                var dungeon = DungeonInfo.GetLargestDungeon(player);
                var oldParty = player.Main.Stats.Party;

                if (player.Location != world)
                {
                    // ���[���h�ɂ��Ȃ��ꍇ

                    // �v���C���[�L�����N�^�[�͕ʋ�ԂɈړ�������B
                    var result = this.Locate(player, null, world, activationDepth);
                    if (!result) return false;

                    // �p�[�e�B�����o�[���ړ�
                    if (oldParty != null)
                    {
                        var oldPartyMembers = oldParty.Members;
                        for (int i = 0; i < oldPartyMembers.Count; i++)
                        {
                            var member = oldPartyMembers[i];
                            if (member == player) continue;
                            if (!this.Locate(member, null, world, activationDepth) && !SpaceUtility.TryLocate(member, world))
                            {
                                Debug.LogError($"{member} �̈ړ��Ɏ��s���܂����B");
                            }
                        }
                    }

                    // �������_���W�����͏����B
                    if (dungeon != null)
                    {
                        dungeon.TrySetStack(0);
                        world.Space.RemoveAllNull();
                    }
                }

                // �p�[�e�B�E���[�_�[�G�t�F�N�g�E���x���A�b�v�{�[�i�X�̏�����
                var party = new RogueParty(player.Main.InfoSet.Faction, player.Main.InfoSet.TargetFactions);
                while (oldParty.Members.Count >= 1)
                {
                    var member = oldParty.Members[0];
                    member.Main.Stats.TryAssignParty(member, party);
                }

                var partyMembers = party.Members;
                for (int i = 0; i < partyMembers.Count; i++)
                {
                    var member = partyMembers[i];
                    DungeonFloorCloserStateInfo.CloseAndRemoveNull(member, true);

                    // ���x����������
                    RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(member, 1);

                    // �T���J�n�O�ɑS�񕜂���
                    member.Main.Stats.Reset(member);

                    member.Main.Stats.Direction = RogueDirection.Down;
                }

                LobbyLeaderEffect.Initialize(player);

                if (player == RogueDevice.Primary.Player)
                {
                    // �v���C���[�L�����Ȃ�ŏ��͏����
                    player.Main.Stats.Direction = RogueDirection.Up;
                }
                else
                {
                    // ����ȊO�͉�����
                    player.Main.Stats.Direction = RogueDirection.Down;
                }

                return true;
            }
        }

        private class AfterLoadRogueMethod : FloorMenuAfterLoadRogueMethod
        {
            public override string GetName(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                return "���r�[";
            }

            public override void Activate(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var world = player.Location;
                var worldInfo = RogueWorldInfo.GetByCharacter(player);
                var tilemap = worldInfo.Lobby.Space.Tilemap;
                var memberInfo = LobbyMemberList.GetMemberInfo(player);
                if (memberInfo.Seat.Location == null)
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
        [ObjectFormer.Formable]
        private class LobbyLeaderEffect : PlayerLeaderRogueEffect, IValueEffect, IRogueObjUpdater
        {
            float IValueEffect.Order => 0f;
            float IRogueObjUpdater.Order => 100f;

            private static readonly LobbyLeaderEffect instance = new LobbyLeaderEffect();
            private static readonly MemberEffect memberEffect = new MemberEffect();
            private LobbyLeaderEffect() { }

            public static void Initialize(RogueObj playerObj)
            {
                var party = playerObj.Main.Stats.Party;
                if (party.Members[0] != playerObj) throw new RogueException(); // ���[�_�[�łȂ���Ύ��s����B

                playerObj.Main.RogueEffects.AddOpen(playerObj, instance);
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
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
            [ObjectFormer.Formable]
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
