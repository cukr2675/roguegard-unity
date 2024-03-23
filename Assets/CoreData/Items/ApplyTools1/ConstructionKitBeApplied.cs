using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Extensions;

namespace Roguegard
{
    public class ConstructionKitBeApplied : BaseApplyRogueMethod
    {
        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var sourceInfoSet = InfoSetSourcedEffect.GetSource(self);
            if (sourceInfoSet == null)
            {
                Debug.LogError($"{self} にオブジェクト情報が設定されていません。");
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);
                return false;
            }
            var space = user.Location.Space;
            if (space.Tilemap == null)
            {
                Debug.LogError($"{user.Location} は {nameof(RogueTilemap)} を持ちません。");
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);
                return false;
            }

            // 床・罠タイルを敷く場合は足元に設置
            var layingPosition = user.Position;

            // 壁タイルを敷く場合は前方に設置
            var sourceTileHasCollider = sourceInfoSet.Ability.HasFlag(MainInfoSetAbility.HasCollider);
            if (sourceTileHasCollider) { layingPosition = user.Position + user.Main.Stats.Direction.Forward; }

            if (!TryGetEquals(sourceInfoSet, layingPosition, user.Location, out var topObj) && space.CollideAt(layingPosition, sourceTileHasCollider))
            {
                // 失敗することが予測される場合は無効
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);
                if (RogueDevice.Primary.Player == user)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "そこには組み立てることができない\n");
                }
                return false;
            }

            if (arg.Count == 0)
            {
                // メニューから使用されたときは工事を開始する

                this.Affect(user, activationDepth, ContinuousApplyStatusEffect.Callback, self);
                return true;
            }
            else if (arg.Count <= 9)
            {
                // 9 ターンまでは何もせずターン経過させる
                return true;
            }
            else
            {
                // 10 ターン経過したらタイルを敷く
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);

                var visible = MainCharacterWorkUtility.VisibleAt(user.Location, user.Position);
                if (topObj?.Main.InfoSet == sourceInfoSet)
                {
                    // すでに同じタイルが敷かれていたら、逆にタイルを消す
                    if (topObj.TrySetStack(0, user))
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, topObj);
                            RogueDevice.Add(DeviceKw.AppendText, "を取り壊した\n");
                        }
                    }
                    else
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, topObj);
                            RogueDevice.Add(DeviceKw.AppendText, "を取り壊せなかった\n");
                        }
                    }
                }
                else
                {
                    var newObj = CharacterCreationInfoSet.CreateObj(sourceInfoSet, user.Location, layingPosition);
                    if (newObj != null)
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, newObj);
                            RogueDevice.Add(DeviceKw.AppendText, "を組み立てた\n");
                        }
                    }
                    else
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, newObj);
                            RogueDevice.Add(DeviceKw.AppendText, "を組み立てられなかった\n");
                        }
                    }
                }
                return true;
            }
        }

        private static bool TryGetEquals(MainInfoSet infoSet, Vector2Int position, RogueObj location, out RogueObj obj)
        {
            var locationObjs = location.Space.Objs;
            for (int i = 0; i < locationObjs.Count; i++)
            {
                obj = locationObjs[i];
                if (obj.Position == position && obj.Main.InfoSet == infoSet) return true;
            }
            obj = null;
            return false;
        }
    }
}
