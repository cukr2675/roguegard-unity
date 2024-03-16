using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using Roguegard;

namespace RoguegardUnity
{
    internal class CharacterRenderSystem
    {
        private RogueSpriteRendererPool pool;

        private Transform parent;

        private List<RogueCharacter> characters;

        public bool InAnimation { get; private set; }

        public bool IsInitialized { get; private set; }

        public void Open(Transform parent, RogueSpriteRendererPool pool)
        {
            var name = "CharacterRenderSystem";
            var characterScreen = new GameObject($"{name} - Parent");
            characterScreen.AddComponent<SortingGroup>();
            this.parent = characterScreen.transform;
            this.parent.SetParent(parent, false);
            this.pool = pool;
            characters = new List<RogueCharacter>();
        }

        public void StartAnimation(RogueObj player)
        {
            IsInitialized = true;

            // 外部へ空間移動したオブジェクトの Position は不定なので参照してはならない。
            // 外部へ空間移動したかを判定するため VisibleObjs は即座に更新する。
            if (ViewInfo.TryGet(player, out var view) && !view.QueueHasItem)
            {
                view.ReadyView(player.Location);
                view.AddView(player);
            }
            InAnimation = true;
        }

        /// <summary>
        /// <see cref="RogueCharacter"/> を更新し、進行中の <see cref="RogueCharacterWork"/> が存在するかを取得する。
        /// </summary>
        public bool UpdateCharactersAndGetWorkingNow(RogueObj player, bool queueDoesNotHaveItems, int deltaTime, bool fastForward)
        {
            if (InAnimation && queueDoesNotHaveItems)
            {
                foreach (var character in characters)
                {
                    character.Ready();
                }
            }

            // 視界内に存在するオブジェクトのスプライトを追加する。
            IRogueTilemapView tilemap;
            if (ViewInfo.TryGet(player, out var view)) { tilemap = view; }
            else { tilemap = player.Location.Space; }

            for (int i = 0; i < tilemap.VisibleObjs.Count; i++)
            {
                var visibleObj = tilemap.VisibleObjs[i];
                if (visibleObj == null || visibleObj.AsTile) continue; // null とタイルはスプライトにしない。

                GetCharacter(visibleObj);
            }

            // 各オブジェクトのアニメーションを進行させる。
            var workingNowAny = false;
            foreach (var character in characters)
            {
                // 動作進行中でなく、見えていないオブジェクトの場合、更新しない。
                if (!character.WorkingNow && view != null && !view.ContainsVisible(character.Obj)) continue;

                // 各オブジェクトのアニメーションを進行させる。
                var speed = fastForward ? 4 : 1;
                character.UpdateCharacter(player, deltaTime * speed);

                // 進行中のアニメーションが存在するかを取得する。
                workingNowAny |= character.WorkingNow;
            }
            return workingNowAny;
        }

        /// <summary>
        /// 指定の <see cref="RogueCharacterWork"/> を再生する。
        /// </summary>
        public void Work(in RogueCharacterWork work, RogueObj player, bool fastForward)
        {
            var character = GetCharacter(work.Obj);
            character.SetWalk(work.Position, work.WalkSpeed, work.Direction, fastForward);
            character.SetSpriteMotion(work.SpriteMotion, work.Continues);
            character.Popup(work.PopSign, work.PopupValue, work.PopupColor, work.PopCritical);
            character.UpdateCharacter(player, 0);
        }

        // 全アニメーション終了後の処理
        public void EndAnimation(RogueObj player, bool completelyEnd)
        {
            if (!InAnimation) return;

            IRogueTilemapView tilemap;
            if (ViewInfo.TryGet(player, out var view)) { tilemap = view; }
            else { tilemap = player.Location.Space; }

            for (int i = characters.Count - 1; i >= 0; i--)
            {
                var character = characters[i];
                if (!character.IsUpdated || // 使用していない要素を取り除く
                    character.Obj == null || // エフェクトを取り除く
                    character.Obj.Location != player.Location || // 別空間の要素を取り除く
                    !tilemap.VisibleObjs.Contains(character.Obj)) // 見えない要素を取り除く
                {
                    pool.PoolCharacter(character);
                    characters.RemoveAt(i);
                }

            }

            // 視界内に存在するオブジェクトのスプライトを追加する。
            for (int i = 0; i < tilemap.VisibleObjs.Count; i++)
            {
                var visibleObj = tilemap.VisibleObjs[i];
                if (visibleObj == null || visibleObj.AsTile) continue; // null とタイルはスプライトにしない。

                visibleObj.Main.Sprite.Update(visibleObj);
                var newCharacter = GetCharacter(visibleObj);
                newCharacter.UpdateCharacter(player, 0);
            }

            foreach (var character in characters)
            {
                // 位置ずれ修正
                // 待機モーションに戻す
                character.EndWorkQueue(completelyEnd);
            }
            InAnimation = false;
        }

        /// <summary>
        /// <see cref="RogueCharacter"/> を取得する。 <see cref="characters"/> に存在しない場合は新規に生成する。
        /// </summary>
        private RogueCharacter GetCharacter(RogueObj obj)
        {
            if (obj == null)
            {
                var newEffect = pool.GetCharacter(parent, null);
                characters.Add(newEffect);
                return newEffect;
            }

            foreach (var character in characters)
            {
                if (character.Obj == obj)
                {
                    //character.
                    return character;
                }
            }

            obj.Main.Sprite.Update(obj); // 初登場はスプライトを更新してから
            var newCharacter = pool.GetCharacter(parent, obj);
            characters.Add(newCharacter);
            return newCharacter;
        }

        /// <summary>
        /// 指定の <see cref="RogueObj"/> のスプライトの位置と方向を取得する。
        /// </summary>
        public bool TryGetPositioning(RogueObj player, out Vector3 position, out RogueDirection direction)
        {
            ViewInfo.TryGet(player, out var view);
            if (view != null && view.Location != player.Location)
            {
                position = default;
                direction = default;
                return false;
            }

            foreach (var character in characters)
            {
                if (character.Obj == player)
                {
                    position = character.transform.position;
                    direction = character.Direction;
                    return true;
                }
            }
            position = default;
            direction = default;
            return false;
        }
    }
}
