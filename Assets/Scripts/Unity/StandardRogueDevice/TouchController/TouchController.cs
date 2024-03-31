using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using UnityEngine.Tilemaps;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// タイルマップのタッチ処理と UI の処理を受け持つクラス。
    /// </summary>
    public class TouchController : MonoBehaviour
    {
        /// <summary>
        /// タイルマップのタッチ処理とボタン UI
        /// </summary>
        [SerializeField] private RogueInputController _inputController = null;

        /// <summary>
        /// メニュー UI
        /// </summary>
        [SerializeField] private MenuController _menuController = null;

        /// <summary>
        /// 画面上部の階層・Lv・HP・MP表示 UI
        /// </summary>
        [SerializeField] private HeaderController _headerController = null;

        private IPathBuilder pointingPathBuilder;
        private WalkStopper pointingWalkStopper;
        private WalkStopper dashForwardWalkStopper;

        internal ModelsMenuEventManager EventManager => _menuController.EventManager;

        /// <summary>
        /// 入力待機状態かを取得する。
        /// </summary>
        public bool WaitsForInput { get; set; }

        public bool InAnimation => WaitsForInput || _menuController.Wait;
        public bool TalkingWait => _menuController.TalkingWait;
        public bool OpenGrid => _inputController.OpenGrid;
        public bool FastForward => _inputController.FastForward;

        private static readonly PushCommand pushCommand = new PushCommand();
        private static readonly SwapPositionCommand swapPositionCommand = new SwapPositionCommand();

        internal void Initialize(
            Tilemap tilemap, SoundController soundController, RogueSpriteRendererPool rendererPool, System.Action stopAutoPlay)
        {
            _inputController.LongPressThresholdTurns = 4;
            _inputController.Initialize(tilemap, false, false, stopAutoPlay);
            _menuController.Initialize(soundController, rendererPool);
            _headerController.Initialize();
        }

        public void OpenWalker(RogueObj player)
        {
            pointingPathBuilder = new AStarPathBuilder(RoguegardSettings.MaxTilemapSize);

            // 画面タッチで移動する場合、敵を発見したとき・敵と隣接しようとしたとき一時停止する
            pointingWalkStopper = new WalkStopper(player);
            pointingWalkStopper.AddStopper(new FoundEnemyWalkStopper());
            pointingWalkStopper.AddStopper(new AdjacentEnemyWalkStopper());

            // 直進移動する場合、敵を発見したとき・敵と隣接しようとしたとき・分岐路にいるとき一時停止する
            dashForwardWalkStopper = new WalkStopper(player);
            dashForwardWalkStopper.AddStopper(new FoundEnemyWalkStopper());
            dashForwardWalkStopper.AddStopper(new AdjacentEnemyWalkStopper());
            dashForwardWalkStopper.AddStopper(new EnteredForkWalkStopper());

            pointingWalkStopper.Initialize();
            dashForwardWalkStopper.Initialize();
        }

        public void MenuOpen(RogueObj subject, bool autoPlayIsEnabled)
        {
            _menuController.Open(subject);
            _inputController.AutoPlayIsEnabled = autoPlayIsEnabled;
        }

        public void GetInfo(out MenuController menuController, out IModelsMenu putIntoChestMenu, out IModelsMenu takeOutFromChestMenu)
        {
            menuController = _menuController;
            _menuController.GetInfo(out putIntoChestMenu, out takeOutFromChestMenu);
        }

        /// <summary>
        /// <see cref="StandardRogueDevice.messageWorkQueue"/> のキュー処理前メソッド
        /// </summary>
        public void EarlyUpdateController(bool visiblePlayer, Vector3 playerPosition, RogueDirection playerDirection, int deltaTime)
        {
            _inputController.EarlyUpdateController(visiblePlayer, playerPosition, playerDirection, deltaTime);

            if (_menuController.Wait)
            {
                // WebGL のタッチ処理で不具合が起きたときのため、メニュー表示中はタッチ状態をリセットする。
                _inputController.ClearTouches();
            }
        }

        /// <summary>
        /// <see cref="StandardRogueDevice.messageWorkQueue"/> のキュー処理後メソッド
        /// </summary>
        public void LateUpdateController(RogueObj player, Vector3 playerPosition, int deltaTime)
        {
            // カメラとメニューを更新する。
            _inputController.LateUpdateController(player, playerPosition);
            _menuController.EventManager.UpdateUI(deltaTime);
        }

        /// <summary>
        /// <see cref="StandardRogueDevice.messageWorkQueue"/> のキュー完全処理後のコマンド入力メソッド
        /// </summary>
        public void CommandProcessing(RogueObj player, RogueObj subject, bool fastForward)
        {
            if (_menuController.Wait) return;

            // メニューにブロックされていないとき、入力情報を更新する。
            Processing(player, subject, fastForward);
            _inputController.CommandUpdate();
        }

        /// <summary>
        /// ターン終わりに一回だけ実行される
        /// </summary>
        public void NextTurn(RogueObj player, RogueObj subject)
        {
            _inputController.NextTurn(player);
            _headerController.UpdateHeader(subject);
        }

        /// <summary>
        /// フィールドとボタン UI のタッチ入力を処理するメソッド
        /// </summary>
        private void Processing(RogueObj player, RogueObj subject, bool fastForward)
        {
            if (!WaitsForInput) return;

            var deviceInfo = RogueDeviceEffect.Get(player);
            if (_inputController.TryGetDirection(fastForward, out var toDirection) && toDirection != player.Main.Stats.Direction)
            {
                //////////////////////////////////////////////////////////////////////////
                // 方向転換
                //////////////////////////////////////////////////////////////////////////
                
                // ボタン UI から方向転換の入力を取得し、現在のプレイヤーキャラの向きと違う場合、プレイヤーキャラを方向転換させる。
                var arg = new RogueMethodArgument(targetPosition: player.Position + toDirection.Forward);
                deviceInfo.SetDeviceCommand(TurnCommandAction.Instance, player, arg);
                EndTurn();
                return;
            }
            if (_inputController.OpenMenu && !_menuController.IsDone)
            {
                //////////////////////////////////////////////////////////////////////////
                // メニューを開く
                //////////////////////////////////////////////////////////////////////////

                // メニュー表示中はボタン UI を消す。
                _inputController.SetEnabled(false);
                _menuController.OpenMainMenu(subject);
                return;
            }
            if (_inputController.GroundIsClick && !_menuController.IsDone)
            {
                //////////////////////////////////////////////////////////////////////////
                // 足元を調べる
                //////////////////////////////////////////////////////////////////////////

                var objs = player.Location.Space.Objs;
                RogueObj groundObj = null;
                for (int i = 0; i < objs.Count; i++)
                {
                    var obj = objs[i];
                    if (obj == null || obj.Position != player.Position || obj == player) continue;

                    if (obj.AsTile) continue;

                    // タイルでなければ拾う対象とする
                    if (groundObj != null)
                    {
                        // 拾う対象が複数存在する場合、拾わずに足元メニューを表示する
                        groundObj = null;
                        break;
                    }
                    groundObj = obj;
                }
                if (groundObj != null)
                {
                    // 足元のアイテムを拾う
                    var device = RogueDeviceEffect.Get(player);
                    var arg = new RogueMethodArgument(tool: groundObj);
                    device.SetDeviceCommand(RoguegardSettings.ObjCommandTable.PickUpCommand, player, arg);
                    EndTurn();
                    return;
                }
                else
                {
                    // メニュー表示中はボタン UI を消す。
                    _inputController.SetEnabled(false);
                    _menuController.OpenGroundMenu(subject);
                    return;
                }
            }
            if (_menuController.IsDone)
            {
                //////////////////////////////////////////////////////////////////////////
                // メニューの確定処理
                //////////////////////////////////////////////////////////////////////////

                // メニューが閉じられたのでボタン UI を再表示する。
                _inputController.SetEnabled(true);
                _menuController.ResetDone();
                EndTurn();
                return;
            }
            if (_inputController.TryGetDashForward(player, out var wait, out var attackTarget, out var startsDash) || wait)
            {
                //////////////////////////////////////////////////////////////////////////
                // ダッシュボタンによる動作
                //////////////////////////////////////////////////////////////////////////

                if (wait)
                {
                    // 足踏み
                    dashForwardWalkStopper.UpdateStatedStop();
                    if (dashForwardWalkStopper.StatedStop && !startsDash)
                    {
                        // ストッパーが発動したら連続動作を停止する
                        _inputController.StopDashForward();
                        return;
                    }

                    deviceInfo.SetDeviceCommand(WaitCommandAction.Instance, player, RogueMethodArgument.Identity);
                    EndTurn();
                    return;
                }
                if (startsDash && attackTarget != null)
                {
                    // 攻撃
                    var arg = new RogueMethodArgument(targetObj: attackTarget, targetPosition: player.Position + player.Main.Stats.Direction.Forward);
                    deviceInfo.SetDeviceCommand(PointAttackCommandAction.Instance, player, arg);
                    EndTurn();
                    return;
                }
                if (AutoWalker.AutoWalking(player, out var targetPosition, dashForwardWalkStopper, startsDash))
                {
                    // 前方へ移動する（通路であれば曲がり角を曲がる）
                    var arg = new RogueMethodArgument(targetPosition: targetPosition);
                    deviceInfo.SetDeviceCommand(WalkCommandAction.Instance, player, arg);
                    EndTurn();
                    return;
                }
                else
                {
                    // ストッパーが発動したら連続動作を停止する
                    _inputController.StopDashForward();

                    // 曲がり角では方向転換だけ行う
                    var arg = new RogueMethodArgument(targetPosition: targetPosition);
                    deviceInfo.SetDeviceCommand(TurnCommandAction.Instance, player, arg);
                    EndTurn();
                    return;
                }
            }
            if (_inputController.TryGetPoint(out var p, out var startsPointing, out var longDown))
            {
                //////////////////////////////////////////////////////////////////////////
                // タイルマップクリックによる動作
                //////////////////////////////////////////////////////////////////////////

                // 長押しはメニュー表示
                if (longDown)
                {
                    // メニュー表示中はボタン UI を消す。
                    _inputController.SetEnabled(false);
                    _menuController.OpenLongDownMenu(subject, p);
                    ClearInput();
                    return;
                }

                // 新規クリック時、移動ルートを生成する。
                if (startsPointing)
                {
                    pointingPathBuilder.UpdatePath(player, p);
                }

                pointingWalkStopper.UpdateStatedStop();
                if (pointingWalkStopper.StatedStop && !startsPointing)
                {
                    // ストッパーが発動したら連続動作を停止する。
                    ClearInput();
                    return;
                }

                if (startsPointing)
                {
                    // オブジェクトを新規クリックした瞬間だけ反応する動作（新規クリックした瞬間だけ攻撃させたい）
                    var view = ViewInfo.Get(player);
                    for (int i = 0; i < view.VisibleObjCount; i++)
                    {
                        var obj = view.GetVisibleObj(i);
                        if (obj == null || obj.Position != p) continue;

                        if (StatsEffectedValues.AreVS(player, obj))
                        {
                            // 敵をクリックして攻撃
                            deviceInfo.SetDeviceCommand(PointAttackCommandAction.Instance, player, new(targetObj: obj, targetPosition: p));
                            EndTurn();
                            ClearInput();
                            return;
                        }
                        else if (obj.Main.InfoSet.Category == CategoryKw.MovableObstacle)
                        {
                            // 大岩をクリックして押して移動させる
                            deviceInfo.SetDeviceCommand(pushCommand, player, new(targetObj: obj));
                            EndTurn();
                            ClearInput();
                            return;
                        }
                        else if (player.Main.Stats.Party.Members.Contains(obj))
                        {
                            // パーティメンバーをクリックして場所を入れ替える
                            deviceInfo.SetDeviceCommand(swapPositionCommand, player, new(targetObj: obj));
                            EndTurn();
                            ClearInput();
                            return;
                        }
                        else if (obj.HasCollider && (p - player.Position).sqrMagnitude <= 2 &&
                            RoguegardSettings.ObjCommandTable.Categories.Contains(obj.Main.InfoSet.Category))
                        {
                            // その他、隣接しているアイテムはコマンドを開く
                            _menuController.OpenLongDownMenu(subject, p);
                            ClearInput();
                            return;
                        }
                    }
                }

                if (pointingPathBuilder.TryGetNextDirection(player, out var direction))
                {
                    var point = player.Position + direction.Forward;
                    pointingWalkStopper.UpdatePositionedStop(point);
                    if (pointingWalkStopper.PositionedStop && !startsPointing)
                    {
                        // ストッパーが発動したら連続動作を停止する。
                        ClearInput();
                        return;
                    }

                    // 地面をクリックして移動
                    deviceInfo.SetDeviceCommand(WalkCommandAction.Instance, player, new(targetPosition: point));
                    EndTurn();
                    return;
                }
                else
                {
                    // ストッパーが発動したら連続動作を停止する。
                    ClearInput();
                    return;
                }
            }

            // 現在のターンの入力を完了し、次のターンまで待つ。
            void EndTurn()
            {
                WaitsForInput = false;
            }
        }

        public void ResetUI()
        {
            _menuController.EventManager.ClearText();
            _inputController.ResetCamera();
        }

        /// <summary>
        /// タイルマップのクリック状態を消す。
        /// </summary>
        public void ClearInput()
        {
            _inputController.ClearInput();
        }

        public void OpenMenu(RogueObj player, IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            _inputController.SetEnabled(false);
            _headerController.UpdateHeader(player);
            _menuController.OpenInitialMenu(menu, self, user, arg);
        }
    }
}
