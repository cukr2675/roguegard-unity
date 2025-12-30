using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// <see cref="InputActionPhase"/> の Flags 版
    /// </summary>
    [System.Flags]
    public enum KeybindPhase
    {
        Disabled = 0,
        //Waiting = 1,
        Started = 2,
        Performed = 4,
        Canceled = 8,
    }
}
