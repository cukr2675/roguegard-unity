using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Extensions;

namespace Roguegard
{
    public class GusSporeBeDefeated : ReferableScript, IAffectRogueMethod
    {
        [SerializeField] private ScriptableStartingItem _dropItem = null;

        private static readonly CommonBeDefeated common = new CommonBeDefeated();
        private static VariantBoneMotion bombMotion;

        private GusSporeBeDefeated() { }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            // 反撃できない場合は普通に倒れる。 10 連爆まで可能
            if (activationDepth >= 10f) return common.Invoke(self, user, activationDepth, arg);
            var nextActivationDepth = Mathf.Floor(activationDepth + 1f);

            // すでに倒れているオブジェクトを倒すことはできない。
            if (self.Stack == 0) return false;

            var lootLocation = self.Location;
            var lootPosition = self.Position;

            if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "は爆発した！\n");
                RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Bomb);
                bombMotion ??= new VariantBoneMotion(CoreMotions.Bomb, new Color32(215, 255, 64, 255));
                RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateEffect(self.Position, bombMotion, false));
            }

            // 消す前に当たり判定
            using var predicator = ForAllRogueMethodTarget.Instance.GetPredicator(self, 0f, null);
            Within1TileRogueMethodRange.Instance.Predicate(predicator, self, 0f, null, lootPosition);

            // 倒れたキャラクターを消す。
            self.TrySetStack(0, user);

            var targets = predicator.GetObjs(lootPosition);
            for (int i = 0; i < targets.Count; i++)
            {
                // 周囲1マスに攻撃力+2ダメージ
                var target = targets[i];
                var dropPosition = target.Position;
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += 2f;
                default(IAffectRogueMethodCaller).TryHurt(target, user, nextActivationDepth, damageValue, true);
                var defeated = default(IAffectRogueMethodCaller).TryDefeat(target, user, nextActivationDepth, damageValue);
                if (!defeated) continue;

                // 爆発で敵を倒したとき、アイテムを生成する。
                _dropItem.Option.CreateObj(_dropItem, lootLocation, dropPosition, RogueRandom.Primary);
            }

            // アイテムドロップ
            WeightedRogueObjGeneratorUtility.CreateObjs(self.Main.InfoSet.LootTable, lootLocation, lootPosition, RogueRandom.Primary);

            return true;
        }
    }
}
