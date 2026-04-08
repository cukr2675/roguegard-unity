using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// <see cref="KeyOptionViewItemHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface IKeyOption<in TMgr>
    {
        string GetName(TMgr manager);

        string GetStyle(TMgr manager);

        void Started(TMgr manager, InputAction.CallbackContext context);
        void Performed(TMgr manager, InputAction.CallbackContext context);
        void Canceled(TMgr manager, InputAction.CallbackContext context);
    }
}
