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
                    // ワールドにいない場合

                    // プレイヤーキャラクターとパーティメンバーは別空間に移動させる。
                    var result = this.LocateWithPartyMembers(player, null, world, activationDepth, true);
                    if (!result) return false;

                    // 元居たダンジョンは消す。
                    if (dungeon != null)
                    {
                        dungeon.TrySetStack(0);
                        world.Space.RemoveAllNull();
                    }
                }

                // パーティ・リーダーエフェクト・レベルアップボーナスの初期化
                var party = new RogueParty(player.Main.InfoSet.Faction, player.Main.InfoSet.TargetFactions);
                RoguePartyUtility.AssignWithPartyMembers(player, party);

                RoguePartyUtility.Reset(party, lobbyLeaderEffect);

                if (player == RogueDevice.Primary.Player)
                {
                    // プレイヤーキャラなら最初は上向き
                    player.Main.Stats.Direction = RogueDirection.Up;
                }

                return true;
            }
        }

        private class AfterLoadRogueMethod : FloorMenuAfterLoadRogueMethod
        {
            protected override string GetName(RogueMenuManager manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                return "ロビー";
            }

            protected override void Activate(RogueMenuManager manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var world = RogueWorldInfo.GetWorld(player);
                var worldInfo = RogueWorldInfo.GetByCharacter(player);
                var tilemap = worldInfo.Lobby.Space.Tilemap;
                var memberInfo = LobbyMemberList.GetMemberInfo(player);
                if (memberInfo.Seat != null && memberInfo.Seat.Location == null)
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
