using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using Roguegard;

namespace RoguegardUnity
{
    public class RogueSpriteRendererPool : MonoBehaviour
    {
        private readonly Stack<SpriteRenderer> pooledSpriteRenderers = new Stack<SpriteRenderer>();
        private readonly Stack<SortingGroup> pooledSortingGroups = new Stack<SortingGroup>();
        private readonly Stack<RogueObjSpriteRenderer> pooledRogueObjSpriteRenderers = new Stack<RogueObjSpriteRenderer>();
        private readonly Stack<RogueCharacter> pooledCharacters = new Stack<RogueCharacter>();

        [SerializeField] private SpriteRenderer _spriteRendererPrefab = null;
        [SerializeField] private RogueCharacter _characterPrefab = null;

        private void Awake()
        {
            if (_spriteRendererPrefab == null) throw new RogueException($"{nameof(_spriteRendererPrefab)} が設定されていません。");
            if (_characterPrefab == null) throw new RogueException($"{nameof(_characterPrefab)} が設定されていません。");
        }

        public void PoolSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            spriteRenderer.transform.SetParent(transform, false);
            spriteRenderer.gameObject.SetActive(false);
            pooledSpriteRenderers.Push(spriteRenderer);
        }

        public void PoolSortingGroup(SortingGroup sortingGroup)
        {
            sortingGroup.transform.SetParent(transform, false);
            sortingGroup.gameObject.SetActive(false);
            pooledSortingGroups.Push(sortingGroup);
        }

        public void PoolRogueSpriteRenderer(RogueObjSpriteRenderer rogueObjSpriteRenderer)
        {
            rogueObjSpriteRenderer.AdjustBones(0);
            rogueObjSpriteRenderer.transform.SetParent(transform, false);
            rogueObjSpriteRenderer.gameObject.SetActive(false);
            pooledRogueObjSpriteRenderers.Push(rogueObjSpriteRenderer);
        }

        public void PoolCharacter(RogueCharacter character)
        {
            character.Destroy();
            character.transform.SetParent(transform, false);
            pooledCharacters.Push(character);
        }

        public SpriteRenderer GetRenderer(Transform parent)
        {
            SpriteRenderer spriteRenderer;
            if (pooledSpriteRenderers.Count >= 1)
            {
                spriteRenderer = pooledSpriteRenderers.Pop();
                spriteRenderer.transform.SetParent(parent, false);
                spriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                spriteRenderer = Instantiate(_spriteRendererPrefab, parent);
            }
            return spriteRenderer;
        }

        public SortingGroup GetSortingGroup(Transform parent)
        {
            SortingGroup sortingGroup;
            if (pooledSortingGroups.Count >= 1)
            {
                sortingGroup = pooledSortingGroups.Pop();
                sortingGroup.transform.SetParent(parent, false);
                sortingGroup.gameObject.SetActive(true);
            }
            else
            {
                var newObject = new GameObject("SortingGroup");
                newObject.transform.SetParent(parent, false);
                sortingGroup = newObject.AddComponent<SortingGroup>();
            }
            return sortingGroup;
        }

        public RogueObjSpriteRenderer GetRogueSpriteRenderer(Transform parent)
        {
            RogueObjSpriteRenderer rogueObjSpriteRenderer;
            if (pooledRogueObjSpriteRenderers.Count >= 1)
            {
                rogueObjSpriteRenderer = pooledRogueObjSpriteRenderers.Pop();
                rogueObjSpriteRenderer.transform.SetParent(parent, false);
                rogueObjSpriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                var newObject = new GameObject("RogueObjSpriteRenderer");
                newObject.transform.SetParent(parent, false);
                rogueObjSpriteRenderer = newObject.AddComponent<RogueObjSpriteRenderer>();
                rogueObjSpriteRenderer.Initialize(this);
            }
            return rogueObjSpriteRenderer;
        }

        public RogueCharacter GetCharacter(Transform parent, RogueObj obj)
        {
            RogueCharacter character;
            if (pooledCharacters.Count >= 1)
            {
                character = pooledCharacters.Pop();
                character.transform.SetParent(parent, false);
            }
            else
            {
                character = Instantiate(_characterPrefab, parent);
            }
            character.Initialize(obj, this);
            return character;
        }
    }
}
