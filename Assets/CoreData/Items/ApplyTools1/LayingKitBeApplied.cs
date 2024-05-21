using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class LayingKitBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private RogueTileLayer _tileLayer = RogueTileLayer.Ground;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var tileVariantInfo = TileReferenceInfo.Get(self);
            if (tileVariantInfo == null || tileVariantInfo.Count == 0)
            {
                Debug.LogError($"{self} にタイルが設定されていません。");
                BaseStatusEffect.Close<ContinuousApplyStatusEffect>(user);
                return false;
            }
            var sourceTile = tileVariantInfo.Get(0);
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
            var overwrite = _tileLayer == RogueTileLayer.Ground;

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

                var visible = MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var handler);
                if (_tileLayer != RogueTileLayer.Ground && topTile.Equals(sourceTile))
                {
                    // すでに同じタイルが敷かれていたら、逆にタイルを消す（床タイルを除く）
                    if (space.TryRemove(layingPosition, sourceTile.Info.Layer))
                    {
                        if (visible)
                        {
                            handler.AppendText(user).AppendText("は").AppendText(topTile).AppendText("を取り壊した\n");
                        }
                    }
                    else
                    {
                        if (visible)
                        {
                            handler.AppendText(user).AppendText("は").AppendText(topTile).AppendText("を取り壊せなかった\n");
                        }
                    }
                }
                else
                {
                    if (space.TrySet(sourceTile, layingPosition, overwrite))
                    {
                        if (visible)
                        {
                            handler.AppendText(user).AppendText("は").AppendText(sourceTile);
                            if (sourceTileHasCollider)
                            {
                                handler.AppendText("を建てた\n");
                            }
                            else
                            {
                                handler.AppendText("を敷いた\n");
                            }
                        }
                    }
                    else
                    {
                        if (visible)
                        {
                            handler.AppendText(user).AppendText("は").AppendText(sourceTile).AppendText("を建てられなかった\n");
                        }
                    }
                }
                handler?.Dispose();
                return true;
            }
        }
    }
}
