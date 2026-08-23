using UnityEngine;

namespace Lysionium.Views
{
    // 命名メモ:
    // EventGestureRule はアイコンなどの拡張性を阻害するのと EventGestureRecognizer と頭辞語 (EGR) が同じになる
    public abstract class EventGestureDefinition : ScriptableObject
    {
        public abstract IEventGestureDefinitionRecognizer CreateDefinitionRecognizer();
    }
}
