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
                    // ワールドにいない場合

                    // プレイヤーキャラクターは別空間に移動させる。
                    var result = this.Locate(player, null, world, activationDepth);
                    if (!result) return false;

                    // パーティメンバーも移動
                    if (oldParty != null)
                    {
                        var oldPartyMembers = oldParty.Members;
                        for (int i = 0; i < oldPartyMembers.Count; i++)
                        {
                            var member = oldPartyMembers[i];
                            if (member == player) continue;
                            if (!this.Locate(member, null, world, activationDepth) && !SpaceUtility.TryLocate(member, world))
                            {
                                Debug.LogError($"{member} の移動に失敗しました。");
                            }
                        }
                    }

                    // 元居たダンジョンは消す。
                    if (dungeon != null)
                    {
                        dungeon.TrySetStack(0);
                        world.Space.RemoveAllNull();
                    }
                }

                // パーティ・リーダーエフェクト・レベルアップボーナスの初期化
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

                    // レベルを初期化
                    RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(member, 1);

                    // 探索開始前に全回復する
                    member.Main.Stats.Reset(member);

                    member.Main.Stats.Direction = RogueDirection.Down;
                }

                LobbyLeaderEffect.Initialize(player);

                if (player == RogueDevice.Primary.Player)
                {
                    // プレイヤーキャラなら最初は上向き
                    player.Main.Stats.Direction = RogueDirection.Up;
                }
                else
                {
                    // それ以外は下向き
                    player.Main.Stats.Direction = RogueDirection.Down;
                }

                return true;
            }
        }

        private class AfterLoadRogueMethod : FloorMenuAfterLoadRogueMethod
        {
            public override string GetName(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                return "ロビー";
            }

            public override void Activate(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var world = player.Location;
                var worldInfo = RogueWorldInfo.GetByCharacter(player);
                var tilemap = worldInfo.Lobby.Space.Tilemap;
                var memberInfo = LobbyMemberList.GetMemberInfo(player);
                if (memberInfo.Seat.Location == null)
                {
                    // 席が消えていたら無効化
                    memberInfo.Seat = null;
                }
                Vector2Int position;
                if (memberInfo.Seat != null && memberInfo.Seat.Location == worldInfo.Lobby)
                {
                    position = memberInfo.Seat.Position;
                }
                else
                {
                    // リスポーン地点が設定されていなければ規定の位置を使用
                    position = new Vector2Int(tilemap.Width / 2, 3);
                }
                if (!SpaceUtility.TryLocate(player, worldInfo.Lobby, position))
                {
                    // 移動に失敗したら壁通過状態で移動させる
                    var movement = MovementCalculator.Get(player);
                    if (!player.TryLocate(worldInfo.Lobby, position, movement.AsTile, false, false, movement.HasSightCollider, StackOption.Default))
                        throw new RogueException("セーブポイントからの復帰に失敗しました。復帰位置に移動できません。");
                }
                // パーティメンバーも移動
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
                            // 席が消えていたら無効化
                            memberMemberInfo.Seat = null;
                        }
                        if (memberMemberInfo.Seat != null && memberMemberInfo.Seat.Location == worldInfo.Lobby)
                        {
                            var memberPosition = memberMemberInfo.Seat.Position;
                            if (SpaceUtility.TryLocate(member, worldInfo.Lobby, memberPosition)) continue;
                        }

                        // リスポーン地点が設定されていなければ規定の位置を使用
                        if (!RoguePartyUtility.TryLocateNextToAnyMember(member, player.Main.Stats.Party))
                        {
                            Debug.LogError($"{member} の移動に失敗しました。");
                        }
                    }
                }

                world.Space.RemoveAllNull();
            }
        }

        /// <summary>
        /// ロビーのプレイヤーパーティメンバーは自然回復する。
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
                if (party.Members[0] != playerObj) throw new RogueException(); // リーダーでなければ失敗する。

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
                // リーダーの自然回復
                self.Main.Stats.Regenerate(self);

                // パーティメンバーに自然回復効果を付与
                memberEffect.AffectToPartyMembersOf(self, false);
                return default;
            }

            public override bool Equals(object obj) => obj.GetType() == GetType();
            public override int GetHashCode() => GetType().GetHashCode();

            /// <summary>
            /// このエフェクトが付与されているオブジェクトは自然回復を有効にする。
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
