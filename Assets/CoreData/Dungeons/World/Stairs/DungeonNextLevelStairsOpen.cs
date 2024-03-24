using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;
using Roguegard.Device;

namespace Roguegard.CharacterCreation
{
    public class DungeonNextLevelStairsOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private DungeonCreationData _data = null;

        private DungeonNextLevelStairsOpen() { }

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var savePoint = new SavePoint(_data);
            SavePointInfo.SetTo(self, savePoint);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            SavePointInfo.RemoveFrom(self);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        [ObjectFormer.Formable]
        private class SavePoint : ISavePointInfo
        {
            private readonly DungeonCreationData data;

            public IApplyRogueMethod BeforeSave => _beforeSave;
            private static readonly IApplyRogueMethod _beforeSave = new BeforeSaveRogueMethod();

            [System.NonSerialized] private IApplyRogueMethod _afterLoad;
            public IApplyRogueMethod AfterLoad => _afterLoad ??= new AfterLoadRogueMethod(data);

            private SavePoint() { }

            public SavePoint(DungeonCreationData data)
            {
                this.data = data;
            }
        }

        private class BeforeSaveRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
            {
                if (LobbyMemberList.GetMemberInfo(player) == null) return false;

                if (MainCharacterWorkUtility.VisibleAt(player.Location, player.Position))
                {
                    RogueDevice.Add(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                }

                // プレイヤーキャラクターは別空間に移動させる。
                var floor = self.Location; // 階段の空間はフロア
                var dungeon = floor.Location; // フロアの空間はダンジョン
                var result = this.Locate(player, null, dungeon, activationDepth);
                if (!result) return false;

                // パーティメンバーも移動
                var party = player.Main.Stats.Party;
                if (party != null)
                {
                    var partyMembers = player.Main.Stats.Party.Members;
                    for (int i = 0; i < partyMembers.Count; i++)
                    {
                        if (partyMembers[i] == player) continue;
                        if (!this.Locate(partyMembers[i], null, dungeon, activationDepth) &&
                            !SpaceUtility.TryLocate(partyMembers[i], dungeon))
                        {
                            Debug.LogError($"{partyMembers[i]} の移動に失敗しました。");
                        }
                    }
                }

                // 階層をレベルとして記憶する。
                dungeon.Main.Stats.SetLv(dungeon, floor.Main.Stats.Lv + 1);

                // 元居たフロアは消す。
                floor.TrySetStack(0);
                dungeon.Space.RemoveAllNull();

                // フロア限定のエフェクトを解除する。
                DungeonFloorCloserStateInfo.CloseAndRemoveNull(player, false);

                // パーティメンバーも解除
                if (party != null)
                {
                    var partyMembers = player.Main.Stats.Party.Members;
                    for (int i = 0; i < partyMembers.Count; i++)
                    {
                        if (partyMembers[i] == player) continue;
                        DungeonFloorCloserStateInfo.CloseAndRemoveNull(partyMembers[i], false);
                    }
                }

                return true;
            }
        }

        private class AfterLoadRogueMethod : FloorMenuAfterLoadRogueMethod
        {
            private readonly DungeonCreationData data;

            public AfterLoadRogueMethod(DungeonCreationData data)
            {
                this.data = data;
            }

            public override string GetName(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var levelText = DungeonInfo.Get(player.Location).GetLevelText(player.Location);
                return $"{data.DescriptionName}\n{levelText}";
            }

            public override void Activate(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var dungeon = player.Location;
                if (!DungeonInfo.TryGet(dungeon, out var info) ||
                    !info.TryGetRandom(dungeon.Main.Stats.Lv, out var random))
                {
                    Debug.LogError("ダンジョンのシード値の取得に失敗しました。");
                    random = new RogueRandom();
                }
                data.StartFloor(player, random);
            }
        }
    }
}
