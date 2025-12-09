using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// <see cref="KeyOptionViewItemHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface IKeyOption<in TMgr, in TArg>
    {
        string GetName(TMgr manager, TArg arg);

        string GetStyle(TMgr manager, TArg arg);

        void Started(TMgr manager, TArg arg, InputAction.CallbackContext context);
        void Canceled(TMgr manager, TArg arg, InputAction.CallbackContext context);
        void Performed(TMgr manager, TArg arg, InputAction.CallbackContext context);
    }
}
