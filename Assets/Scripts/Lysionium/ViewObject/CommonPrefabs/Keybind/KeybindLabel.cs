using UnityEngine;

namespace Lysionium.Views
{
    public abstract class KeybindLabel : MonoBehaviour
    {
        public abstract void SetStyle(System.ReadOnlySpan<char> style);

        public abstract void ResetStyle(System.ReadOnlySpan<char> style);
    }
}
