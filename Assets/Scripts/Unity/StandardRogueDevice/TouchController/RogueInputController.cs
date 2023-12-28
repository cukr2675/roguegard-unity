using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using Roguegard;

namespace RoguegardUnity
{
    /// <summary>
    /// タイルマップのタッチ処理とボタン UI とカメラ操作
    /// </summary>
    public class RogueInputController : MonoBehaviour
    {
        [SerializeField] private AnchorButton _leftAnchorButton = null;
        [SerializeField] private AnchorButton _rightAnchorButton = null;
        [SerializeField] private TouchField _touchField = null;
        [SerializeField] private CameraController _cameraController = null;

        [Space]
        [SerializeField] private RogueDirectionalSpriteTable _sprintSprite = null;
        [SerializeField] private RogueDirectionalSpriteTable _attackSprite = null;
        [SerializeField] private RogueDirectionalSpriteTable _walkSprite = null;
        [SerializeField] private RogueDirectionalSpriteTable _turnSprite = null;
        [SerializeField] private Sprite _waitSprite = null;
        [SerializeField] private Sprite _cameraSprite = null;
        [SerializeField] private Sprite _stopSprite = null;

        [Space]
        [SerializeField] private Sprite _menuSprite = null;
        [SerializeField] private Sprite _groundSprite = null;

        private AnchorButton dashButton;
        private AnchorButton menuButton;
        private bool dashForward;
        private int dashForwardTurns;
        private RogueDirectionalSpriteTable currentDashSpriteTable;
        private System.Action stopAutoPlay;

        public bool OpenMenu => menuButton.IsClick;
        public bool GroundIsClick => menuButton.BalloonIsClick;
        public bool OpenGrid => menuButton.DragKeyIsVisible && !dashButton.IsHeldDown;
        public bool FastForward => _touchField.FastForward || (dashButton.IsHeldDown && dashForwardTurns >= LongPressThresholdTurns);

        public int LongPressThresholdTurns { get; set; }
        public bool AutoPlayIsEnabled { get; set; }

        public void Initialize(Tilemap tilemap, bool dashButtonIsOnTheRight, bool dashKeyMode, System.Action stopAutoPlay)
        {
            if (dashButtonIsOnTheRight)
            {
                dashButton = _rightAnchorButton;
                menuButton = _leftAnchorButton;
            }
            else
            {
                dashButton = _leftAnchorButton;
                menuButton = _rightAnchorButton;
            }

            dashButton.Key = KeyCode.Space;
            dashButton.Initialize(dashKeyMode);
            currentDashSpriteTable = _sprintSprite;
            menuButton.Key = KeyCode.LeftShift;
            menuButton.DragToShowKeys = true;
            menuButton.Initialize(false, _turnSprite);

            _touchField.TouchResetTime = .1f;
            _touchField.ZoomClippingRadius = .2f;
            _touchField.WheelZoomingFilters = new[]
            {
                -4f, -3.5f, -3f, -2.5f, -2f, -1.5f, -1f, -.5f,
                Mathf.Log(1f, 2f), Mathf.Log(1.5f, 2f), Mathf.Log(2f, 2f), Mathf.Log(3f, 2f), Mathf.Log(4f, 2f), Mathf.Log(6f, 2f), Mathf.Log(8f, 2f)
            };
            _touchField.MaxPowedZoom = 8f;
            _touchField.MinPowedZoom = Mathf.Pow(2f, -4f);
            _touchField.LongDownSeconds = .5f;
            _touchField.Initialize(tilemap, _cameraController);

            var screenSize = new Vector2Int(1920, 1080);
            _cameraController.Initialize(screenSize);

            this.stopAutoPlay = stopAutoPlay;
        }

        /// <summary>
        /// タイルマップタッチとボタン UI の処理
        /// </summary>
        public void EarlyUpdateController(bool visiblePlayer, Vector3 playerPosition, RogueDirection playerDirection, int deltaTime)
        {
            // ダッシュボタンクリックで自動モードを終了させる。
            if (dashButton.IsClick && AutoPlayIsEnabled && !_cameraController.IsCameraMode)
            {
                AutoPlayIsEnabled = false;
                stopAutoPlay();
            }

            // ダッシュボタンクリックでカメラモードを終了させる。
            if (dashButton.IsClick && _cameraController.IsCameraMode)
            {
                _cameraController.TerminateCameraMode();
                dashButton.CommandUpdate();
            }

            if (dashButton.IsDown) { dashForward = true; }
            if (!dashButton.IsHeldDown) { dashForward = false; }

            // 前進・足踏み中にメニューボタンの押し離しがあったら停止（足踏みしない）
            if (menuButton.IsDown || menuButton.IsUp) { dashForward = false; }

            // ダッシュボタン長押しのカウントをリセットする。
            if (!dashForward) { dashForwardTurns = 0; }

            // ダッシュボタンかメニューボタンに触れたらポインティングをクリアする。
            if (dashButton.IsHeldDown || menuButton.IsDown) { _touchField.ClearPointing(); }

            // タイルマップタッチ処理（クリック・ドラッグ・ピンチイン・ピンチアウト）
            _touchField.UpdateField(visiblePlayer, playerPosition, playerDirection, deltaTime);

            // 操作状態によってダッシュボタンの絵柄を変える。
            if (_cameraController.IsCameraMode)
            {
                // カメラモード
                dashButton.SetSprite(_cameraSprite);
            }
            else if (AutoPlayIsEnabled)
            {
                dashButton.SetSprite(_stopSprite);
            }
            else if (visiblePlayer)
            {
                if (menuButton.DragKeyIsVisible)
                {
                    if (currentDashSpriteTable == _attackSprite)
                    {
                        // 攻撃
                        dashButton.SetSprite(currentDashSpriteTable.GetSprite(playerDirection));
                    }
                    else
                    {
                        // 歩き
                        dashButton.SetSprite(_walkSprite.GetSprite(playerDirection));
                    }
                }
                else if (menuButton.IsHeldDown)
                {
                    // 足踏み
                    dashButton.SetSprite(_waitSprite);
                }
                else
                {
                    // 攻撃・その他方向表示
                    dashButton.SetSprite(currentDashSpriteTable.GetSprite(playerDirection));
                }
            }
            else
            {
                // それ以外はグレーアウト
                dashButton.SetSprite(null);
            }

            menuButton.SetSprite(_menuSprite);
        }

        public void LateUpdateController(RogueObj player, Vector3 playerPosition)
        {
            _cameraController.UpdateCamera(
                player, playerPosition, _touchField.Drag,
                _touchField.StartsDrag, _touchField.DragRelativePosition, _touchField.PowedZoom, _touchField.DeltaPosition);
            _touchField.ResetStartsDrag();
        }

        public void CommandUpdate()
        {
            // ダッシュボタン長押しのカウントを進める。
            if (dashForward) { dashForwardTurns++; }

            dashButton.CommandUpdate();
            menuButton.CommandUpdate();
        }

        public void NextTurn(RogueObj player)
        {
            // ダッシュボタンを攻撃ボタンにする
            var attackTarget = PointAttackCommandAction.GetVisibleTarget(player);
            if (attackTarget != null) { currentDashSpriteTable = _attackSprite; }
            else { currentDashSpriteTable = _sprintSprite; }

            // 足元に何かあったら吹き出しボタンを表示する
            var view = player.Get<ViewInfo>();
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || obj.Position != player.Position || obj == player) continue;

                menuButton.SetBalloon(true, _groundSprite);
                return;
            }
            menuButton.SetBalloon(false, null);
        }

        public void SetEnabled(bool enabled)
        {
            dashButton.SetEnabled(enabled);
            menuButton.SetEnabled(enabled);
            _touchField.enabled = enabled;
        }

        public void ResetCamera()
        {
            _cameraController.TerminateCameraMode();
        }

        public void ClearInput()
        {
            _touchField.ClearPointing();

            // FastForward を即座に解除する
            dashForwardTurns = 0;
        }

        /// <summary>
        /// ボタンによる方向転換を取得する。
        /// </summary>
        public bool TryGetDirection(bool fastForward, out RogueDirection direction)
        {
            if (fastForward)
            {
                // 早送り中は方向転換を無効化する。
                direction = default;
                return false;
            }

            // 方向転換はメニューボタン優先
            if (menuButton.TryGetDirection(out direction)) return true;
            if (menuButton.TryGetDirection(out direction)) return true;

            direction = default;
            return false;
        }

        public bool TryGetPoint(out Vector2Int point, out bool startsPointing, out bool longDown)
        {
            if (_touchField.IsPointing)
            {
                point = _touchField.PointingPosition;
                startsPointing = _touchField.IsClick;
                longDown = _touchField.IsLongDown;
                _touchField.ResetClick(); // 少なくとも一回は処理をしてからリセットする
                return true;
            }
            else
            {
                point = default;
                startsPointing = default;
                longDown = default;
                return false;
            }
        }

        public bool TryGetDashForward(RogueObj player, out bool wait, out RogueObj attackTarget, out bool startsDashForward)
        {
            if (AutoPlayIsEnabled)
            {
                wait = true;
                attackTarget = null;
                startsDashForward = true;
                return true;
            }

            wait = false;
            if (dashForward && menuButton.IsHeldDown)
            {
                if (menuButton.TryGetDirection(out _))
                {
                    // 方向入力 + ダッシュで歩き
                    dashForwardTurns = -1;
                }
                else
                {
                    // メニュー押し続け + ダッシュで足踏み
                    wait = true;
                }

                // 歩き・足踏みが入力されたらメニューボタンを開かない
                menuButton.ReadyToClick = false;
            }

            if (dashForward && !_cameraController.IsCameraMode)
            {
                attackTarget = PointAttackCommandAction.GetVisibleTarget(player);
                startsDashForward = dashButton.IsDown;
                return true;
            }
            else
            {
                attackTarget = null;
                startsDashForward = false;
                return false;
            }
        }

        public void StopDashForward()
        {
            dashForward = false;
        }

        public void ClearTouches()
        {
            _touchField.ClearTouches();
        }
    }
}
