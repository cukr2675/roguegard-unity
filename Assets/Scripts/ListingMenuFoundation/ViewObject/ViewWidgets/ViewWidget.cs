using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ListingMF
{
    public abstract class ViewWidget : MonoBehaviour
    {
        private Selectable[] selectables;
        private Navigation[] notBlockedNavigations;

        protected internal bool IsBlocked { get; private set; }

        protected virtual ElementsSubViewBase Parent => null;

        public abstract bool TryInstantiateWidget(
            ElementsSubViewBase elementsSubView, IElementHandler handler, object element, out ViewWidget viewWidget);

        public virtual void SetBlock(bool block)
        {
            if (IsBlocked == block) return;
            IsBlocked = block;

            if (selectables == null)
            {
                selectables = GetComponentsInChildren<Selectable>();
                notBlockedNavigations = new Navigation[selectables.Length];
            }

            if (block)
            {
                var selectedObj = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
                for (int i = 0; i < selectables.Length; i++)
                {
                    notBlockedNavigations[i] = selectables[i].navigation;
                    selectables[i].navigation = new Navigation() { mode = Navigation.Mode.None };
                    if (selectables[i].gameObject == selectedObj) { EventSystem.current.SetSelectedGameObject(null); }
                }
            }
            else
            {
                for (int i = 0; i < selectables.Length; i++)
                {
                    selectables[i].navigation = notBlockedNavigations[i];
                }
            }
        }

        public void PlayString(string value) => Parent.PlayFromElement(value, this);
        public void PlayObject(Object value) => Parent.PlayFromElement(value, this);
    }
}
