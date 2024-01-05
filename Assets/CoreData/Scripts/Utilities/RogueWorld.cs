using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public static class RogueWorld
    {
        public static ISavePointInfo SavePointInfo { get; } = new SavePoint();

        public static void SetUpWorld(RogueObj world, RogueObj lobby)
        {
            var info = new Info(lobby);
            world.SetInfo(info);
        }

        public static RogueObj GetWorld(RogueObj self)
        {
            var location = self.Location;
            while (true)
            {
                if (location == null) throw new RogueException("ワールドにあたるオブジェクトが存在しません。");
                if (location.TryGet<Info>(out _)) return location;

                location = location.Location;
            }
        }

        public static RogueObj GetLobbyByCharacter(RogueObj self)
        {
            var world = GetWorld(self);
            if (world.TryGet<Info>(out var info))
            {
                return info.Lobby;
            }
            else
            {
                return null;
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

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            public RogueObj Lobby { get; }

            public bool IsExclusedWhenSerialize => false;

            [ObjectFormer.CreateInstance]
            private Info() { }

            public Info(RogueObj lobby)
            {
                Lobby = lobby;
            }

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return null;
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }




        [ObjectFormer.Formable]
        private class SavePoint : ISavePointInfo
        {
            public IApplyRogueMethod BeforeSave => _beforeSave;
            private static readonly IApplyRogueMethod _beforeSave = new BeforeSaveRogueMethod();

            public IApplyRogueMethod AfterLoad => _afterLoad;
            private static readonly IApplyRogueMethod _afterLoad = new AfterLoadRogueMethod();
        }

        private class BeforeSaveRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
            {
                if (!LobbyMembers.Contains(player)) return false;

                var world = GetWorld(player);
                var dungeon = DungeonInfo.GetLargestDungeon(player);

                if (player.Location != world)
                {
                    // ワールドにいない場合

                    // プレイヤーキャラクターは別空間に移動させる。
                    var result = this.Locate(player, null, world, activationDepth);
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
                player.Main.Stats.TryAssignParty(player, party);
                DungeonFloorCloserStateInfo.CloseAndRemoveNull(player, true);
                LobbyLeaderEffect.Initialize(player);
                RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(player, 1);

                // 探索開始前に全回復する
                player.Main.Stats.Reset(player);

                // 最初は上向き
                player.Main.Stats.Direction = RogueDirection.Up;

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
                var lobby = GetLobbyByCharacter(player);
                var tilemap = lobby.Space.Tilemap;
                var memberInfo = LobbyMembers.GetMemberInfo(player);
                Vector2Int position;
                if (memberInfo.Seat != null && memberInfo.Seat.Location == lobby)
                {
                    position = memberInfo.Seat.Position;
                }
                else
                {
                    // リスポーン地点が設定されていなければ規定の位置を使用
                    position = new Vector2Int(tilemap.Width / 2, 3);
                }
                if (!SpaceUtility.TryLocate(player, lobby, position))
                {
                    // 移動に失敗したら壁通過状態で移動させる
                    var movement = MovementCalculator.Get(player);
                    if (!player.TryLocate(lobby, position, movement.AsTile, false, false, movement.HasSightCollider, StackOption.Default))
                        throw new RogueException("セーブポイントからの復帰に失敗しました。復帰位置に移動できません。");
                }

                world.Space.RemoveAllNull();
            }
        }
    }
}
