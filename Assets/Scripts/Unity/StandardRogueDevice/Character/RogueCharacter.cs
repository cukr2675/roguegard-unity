using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard;

namespace RoguegardUnity
{
    public class RogueCharacter : MonoBehaviour
    {
        [SerializeField] private RogueCharacterSpriteRenderer _spriteRenderer = null;
        [SerializeField] private RogueCharacterCanvas _canvas = null;

        public RogueObj Obj { get; private set; }
        public bool WorkingNow { get; private set; }
        public bool IsUpdated { get; private set; }

        private Vector3 startWalkPosition;
        private Vector3 endWalkPosition;
        private Vector2Int endWalkPositionSource;
        private int walkTime;
        private float walkTimeLength;
        private int boneMotionAnimationTime;
        private int motionEffectAnimationTime;
        public RogueDirection Direction { get; private set; }

        private IBoneMotion currentBoneMotion;
        private bool boneMotionIsClosed;

        public void Initialize(RogueObj self, RogueSpriteRendererPool pool)
        {
            name = self?.Main.InfoSet.Name ?? "Effect";
            _spriteRenderer.Initialize(pool);
            _canvas.Initialize(self);
            boneMotionAnimationTime = 0;
            motionEffectAnimationTime = 0;

            if (self != null)
            {
                Obj = self;
                SetWalk(self.Position, Mathf.Infinity, self.Main.Stats.Direction, true);
            }
            else
            {
                SetWalk(Vector2Int.zero, Mathf.Infinity, RogueDirection.Down, true);
            }
            SetBoneMotion(KeywordBoneMotion.Wait, true);
            boneMotionIsClosed = false;
            WorkingNow = false;
        }

        public void Destroy()
        {
            Obj = null;
            _spriteRenderer.Destroy();
        }

        public void Ready()
        {
            IsUpdated = false;
        }

        public void SetWalk(Vector2Int position, float walkSpeed, RogueDirection direction, bool fastForward)
        {
            Direction = direction;

            // 速さがゼロのときは何もしない。
            if (walkSpeed == 0f) return;

            var newPosition = new Vector3(position.x + .5f, position.y + .25f, -.9f);
            if (float.IsPositiveInfinity(walkSpeed))
            {
                // 速さが無限大のとき一瞬で移動させる。
                endWalkPosition = newPosition;
                endWalkPositionSource = position;
                walkTime = 1;
                walkTimeLength = 1f;
                return;
            }

            if (WorkingNow)
            {
                // 動作中に移動先を設定したとき
                var walkTimeRatio = walkTime / walkTimeLength;
                startWalkPosition = Vector3.Lerp(startWalkPosition, endWalkPosition, walkTimeRatio);
            }
            else
            {
                startWalkPosition = endWalkPosition;
            }
            endWalkPosition = newPosition;
            endWalkPositionSource = position;

            if (fastForward)
            {
                // 早送りのときは非動作扱い
                walkTime = 1;
                walkTimeLength = 1f;
                WorkingNow = false;
            }
            else
            {
                walkTime = 0;
                if (walkSpeed > 0f)
                {
                    // 速さがゼロより大きいとき、始点から終点までをその速さで移動させる。
                    var distance = (endWalkPosition - startWalkPosition).magnitude;
                    walkTimeLength = distance / walkSpeed;
                    if (walkTimeLength == 0f)
                    {
                        // 移動前と移動先が同じ場合、一瞬で移動させてゼロ除算を回避する。
                        walkTime = 1;
                        walkTimeLength = 1f;
                        return;
                    }
                }
                else // if (walkSpeed < 0f)
                {
                    // 速さがゼロより小さいとき、始点から終点までをその時間をかけて移動させる。
                    // -1 のとき通常移動
                    walkTimeLength = -walkSpeed;
                }
                walkTimeLength *= 60f;
                WorkingNow = true;
            }
        }

        public void SetBoneMotion(IBoneMotion boneMotion, bool continues)
        {
            if (boneMotion == null || boneMotion == currentBoneMotion) return;

            currentBoneMotion = boneMotion;
            boneMotionAnimationTime = 0;
            if (!continues) { WorkingNow = true; }
        }

        public void Popup(RogueCharacterWork.PopSignType sign, int number, Color color, bool critical)
        {
            _canvas.Popup(sign, number, color, critical);
        }

        /// <summary>
        /// このインスタンスの動作終了処理
        /// </summary>
        public void EndWorkQueue(bool completelyEnd)
        {
            // 待機モーションに切り替えるフラグを設定する。
            // モーションが終端のときだけ切り替える。
            boneMotionIsClosed = true;

            if (Obj == null) return;

            // 位置のずれを直す。
            if (endWalkPositionSource != Obj.Position)
            {
                //Debug.LogWarning("オブジェクトの表示位置が違います。");
                SetWalk(Obj.Position, Mathf.Infinity, Obj.Main.Stats.Direction, true);
            }
            if (Direction != Obj.Main.Stats.Direction)
            {
                SetWalk(Obj.Position, Mathf.Infinity, Obj.Main.Stats.Direction, true);
            }
            walkTime = Mathf.CeilToInt(walkTimeLength);
            if (completelyEnd) { SetBoneMotion(KeywordBoneMotion.Wait, true); }

            // HP のずれを直す。
            _canvas.SetHP(Obj);
        }

        public void UpdateCharacter(RogueObj player, int deltaTime)
        {
            // 位置
            walkTime += deltaTime;
            var walkTimeRatio = walkTime / walkTimeLength;
            var localPosition = Vector3.Lerp(startWalkPosition, endWalkPosition, walkTimeRatio);
            localPosition.x = Mathf.Round(localPosition.x * 32f) / 32f;
            localPosition.y = Mathf.Round(localPosition.y * 32f) / 32f;
            transform.localPosition = localPosition;
            var endOfWalk = walkTimeRatio >= 1f;
            if (endOfWalk && currentBoneMotion.Keyword == new BoneMotionKeyword(MainInfoKw.Walk.Name))
            {
                // 位置変更終了で動作完了
                // 歩きモーションのときだけモーション終了前に前もって動作完了させる。
                WorkingNow = false;
            }

            // スプライト
            boneMotionAnimationTime += deltaTime;
            motionEffectAnimationTime += deltaTime;
            bool endOfMotion;
            if (Obj != null)
            {
                _spriteRenderer.SetSprite(Obj, currentBoneMotion, boneMotionAnimationTime, motionEffectAnimationTime, Direction, out endOfMotion);
            }
            else
            {
                _spriteRenderer.SetEffectSprite(localPosition, currentBoneMotion, motionEffectAnimationTime, Direction, out endOfMotion);
            }
            if (endOfWalk && endOfMotion)
            {
                if (boneMotionIsClosed)
                {
                    // モーションが終端のときだけ待機モーションに切り替える。（待機系モーションの場合は切り替えない）
                    if (currentBoneMotion.Keyword != new BoneMotionKeyword(MainInfoKw.Wait.Name)) SetBoneMotion(KeywordBoneMotion.Wait, true);
                    boneMotionIsClosed = false;
                }

                // 位置変更とモーションが終端に達したとき動作完了させる。
                // TrySetBoneMotion より後に設定する。
                WorkingNow = false;
            }

            // UI
            if (Obj != null) { _canvas.UpdateCanvas(Obj, player, deltaTime); }

            IsUpdated = true;
        }
    }
}
