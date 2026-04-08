using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lysionium.Samples
{
    public class DataBindingSample : MonoBehaviour
    {
        [SerializeField] private ExMgr _manager = null;

        // ダイアログ表示中にも追加・削除・変更できるようにするため ViewItem のキーバインドは使わない
        [Header("Input Actions")]
        [SerializeField] private InputActionReference _add = null;
        [SerializeField] private InputActionReference _remove = null;
        [SerializeField] private InputActionReference _change = null;

        private InputAction Add => _add;
        private InputAction Remove => _remove;
        private InputAction Change => _change;

        private BindingList<BindingValue> list;
        private int addIndex;

        protected virtual void Start()
        {
            _manager.Initialize();

            list = new BindingList<BindingValue>
            {
                new("Tabキーで追加"),
                new("Deleteキーで削除"),
                new("Spaceキーで変更"),
            };

            _manager.PushInitialScreen(new MainScreen(list));
        }

        protected virtual void OnEnable()
        {
            Add.performed += Add_performed;
            Remove.performed += Remove_performed;
            Change.performed += Change_performed;

            Add.Enable();
            Remove.Enable();
            Change.Enable();
        }

        protected virtual void OnDisable()
        {
            Add.performed -= Add_performed;
            Remove.performed -= Remove_performed;
            Change.performed -= Change_performed;

            Add.Disable();
            Remove.Disable();
            Change.Disable();
        }

        private void Add_performed(InputAction.CallbackContext obj)
        {
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>()) return;

            list.Add(new BindingValue($"Added {addIndex}"));
            addIndex++;
        }

        private void Remove_performed(InputAction.CallbackContext obj)
        {
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>()) return;

            if (list.Count >= 1)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        private void Change_performed(InputAction.CallbackContext obj)
        {
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>()) return;

            if (list.Count >= 1)
            {
                list[^1].Age++;
            }
        }

        private class MainScreen : IListuiScreen<ExMgr>
        {
            private readonly BindingList<BindingValue> list;
            private readonly BindableScrollMenuViewData<BindingValue, ExMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public MainScreen(BindingList<BindingValue> list)
            {
                this.list = list;
            }

            public void OpenScreen(ExMgr manager)
            {
                view.Show(list, manager)
                    ?
                    .VarOnceRebindList(out BindingList<BindingValue>.OnChangedHandler rebindList, rebindList => () => rebindList(list))
                    .OnShow(_ => list.OnChanged += rebindList)
                    .OnHide(_ => list.OnChanged -= rebindList)
                    .BinderFrom(notify => new BindingValueDataBinder(notify))

                    // 項目クリック時、削除ダイアログ表示
                    .VarOnce(out BindingValue selectedItem)
                    .VarOnce(
                        out var removeDialog, new ChoicesScreen<ExMgr>(_ => $"{selectedItem} を削除しますか？")
                        .Option("削除", (manager) =>
                        {
                            list.Remove(selectedItem);
                            manager.PopScreen();
                        })
                        .Back())
                    .OnClick((item, manager) =>
                    {
                        selectedItem = item;
                        manager.PushScreen(removeDialog);
                    })

                    // 追加ボタン押下時、追加ダイアログ表示
                    .Tail.Option("+ 追加", new AddDialogScreen(name => list.Add(new BindingValue(name))))

                    .Build();
            }
        }

        private class AddDialogScreen : IListuiScreen<ExMgr>
        {
            private readonly DialogViewData<ExMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            private readonly System.Action<string> onOk;

            public bool IsIncremental => true;

            public AddDialogScreen(System.Action<string> onOk)
            {
                this.onOk = onOk;
            }

            public void OpenScreen(ExMgr manager)
            {
                view.Show("名前を入力してください", manager)
                    ?
                    // name 入力欄
                    .VarOnce(out var name, "")
                    .Tail.Append(InputFieldWidgetOption.Create<ExMgr>(
                        value: _ => name,
                        handleValueChanged: value => name = value))

                    .Tail.Append(StackWidgetOption.Create(

                        // OK ボタン押下時、 name を引数としてコールバック実行
                        ("1*", SelectOption.Create<ExMgr>("登録", (manager) =>
                        {
                            onOk?.Invoke(name);
                            manager.PopScreen();
                        })),

                        // 戻るボタン
                        ("1*", BackSelectOption<ExMgr>.Instance)))

                    .Build();
            }

            public void CloseScreenView(ExMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
