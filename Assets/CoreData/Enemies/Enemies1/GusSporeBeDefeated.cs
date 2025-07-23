using OchalikeSprites;
using Roguegard.CharacterCreation;
using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard
{
    public class GusSporeBeDefeated : ReferableScript, IAffectRogueMethod
    {
        [SerializeField] private AssetStartingItem _dropItem = null;

        private static readonly CommonBeDefeated common = new();
        private static VariantSpriteMotion bombMotion;

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

            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                bombMotion ??= new VariantSpriteMotion(CoreMotions.Bomb, new Color32(215, 255, 64, 255));
                using var handler = h;
                handler.AppendText(self).AppendText("は爆発した！\n");
                handler.EnqueueSE(StdKw.Bomb);
                handler.EnqueueWork(RogueCharacterWork.CreateEffect(self.Position, bombMotion, false));
            }

            // 消す前に当たり判定
            using var predicator = ForAllRogueMethodTarget.Instance.GetPredicator(self, 0f, null);
            Within1TileRogueMethodRange.Instance.Predicate(predicator, self, 0f, null, lootPosition);

            // 倒れたキャラクターを消す。
            self.TrySetStack(0, user);

            foreach (var target in predicator.GetObjs(lootPosition))
            {
                // 周囲1マスに攻撃力+2ダメージ
                var dropPosition = target.Position;
                using var damageValue = EffectableValue.Get();
                StatsEffectedValues.GetAtk(self, damageValue);
                damageValue.MainValue += 2f;
                default(IAffectRogueMethodCaller).TryHurt(target, user, AttackUtility.GetActivationDepthCantCounter(nextActivationDepth), damageValue);
                var defeated = default(IAffectRogueMethodCaller).TryDefeat(target, user, nextActivationDepth, damageValue);
                if (!defeated) continue;

                // 爆発で敵を倒したとき、アイテムを生成する。
                if (_dropItem.Option != null) { _dropItem.Option.CreateObj(_dropItem, lootLocation, dropPosition, RogueRandom.Primary); }
            }

            // アイテムドロップ
            WeightedRogueObjGeneratorUtility.CreateObjs(self.Main.InfoSet.LootTable, lootLocation, lootPosition, RogueRandom.Primary);

            return true;
        }
    }
}
