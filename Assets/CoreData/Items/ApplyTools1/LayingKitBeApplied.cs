using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class LayingKitBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private RogueTileLayer _tileLayer = RogueTileLayer.Floor;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var sourceTile = TileSourcedEffect.GetSource(self);
            if (sourceTile == null)
            {
                Debug.LogError($"{self} にタイルが設定されていません。");
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);
                return false;
            }
            if (sourceTile.Info.Layer != _tileLayer)
            {
                Debug.LogError($"{self.Main.InfoSet.Name} で {_tileLayer} 以外を敷くことはできません。");
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
            var sourceTileHasCollider = sourceTile.Info.HasCollider;
            if (sourceTileHasCollider) { layingPosition = user.Position + user.Main.Stats.Direction.Forward; }

            // 床タイルのみ上書き可能
            var overwrite = _tileLayer == RogueTileLayer.Floor;

            var topTile = space.Tilemap.GetTop(layingPosition);
            if (!topTile.Equals(sourceTile) && space.TileCollideAt(layingPosition, _tileLayer, sourceTileHasCollider, overwrite))
            {
                // 失敗することが予測される場合は無効
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);
                if (RogueDevice.Primary.Player == user)
                {
                    if (sourceTileHasCollider)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "そこには建てることができない\n");
                    }
                    else
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "そこには敷くことができない\n");
                    }
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
                if (_tileLayer != RogueTileLayer.Floor && topTile.Equals(sourceTile))
                {
                    // すでに同じタイルが敷かれていたら、逆にタイルを消す（床タイルを除く）
                    if (space.TryRemove(layingPosition, sourceTile.Info.Layer))
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, topTile);
                            RogueDevice.Add(DeviceKw.AppendText, "を取り壊した\n");
                        }
                    }
                    else
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, topTile);
                            RogueDevice.Add(DeviceKw.AppendText, "を取り壊せなかった\n");
                        }
                    }
                }
                else
                {
                    if (space.TrySet(sourceTile, layingPosition, overwrite))
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, sourceTile);
                            if (sourceTileHasCollider)
                            {
                                RogueDevice.Add(DeviceKw.AppendText, "を建てた\n");
                            }
                            else
                            {
                                RogueDevice.Add(DeviceKw.AppendText, "を敷いた\n");
                            }
                        }
                    }
                    else
                    {
                        if (visible)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, user);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, sourceTile);
                            RogueDevice.Add(DeviceKw.AppendText, "を建てられなかった\n");
                        }
                    }
                }
                return true;
            }
        }
    }
}
