using UnityEngine;

namespace Lysionium
{
    // 命名メモ:
    // UiManager や UiScreen, Uiscreen にすると Lysionium List-UI と無関係な Ui○○ との区別が難しくなる（「UI Toolkit の画面」にミスリードする）
    // ListUiManager にすると HudListUiScreen となって HudList の UiScreen に見えてしまう
    // ListmenuManager にすると HudListmenuScreen となって「HudList のメニュー」にミスリードしやすい
    // LuiManager にすると List-UI と直接関係しない Lysionium の機能と区別が難しくなる

    public interface IListuiManager
    {
        ISelectOption<IListuiManager, IListuiArg> ErrorOption { get; }

        event System.Action OnUnload;

        void HideAll(bool back);

        void SetInvisibleDropdownPosition(Rect rect);

        string Localize(string text);
    }
}
