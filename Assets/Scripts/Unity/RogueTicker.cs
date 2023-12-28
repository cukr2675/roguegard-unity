using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    public class RogueTicker : MonoBehaviour
    {
        [SerializeField] private bool _stop = false;

        private bool updating;

        private TickEnumerator tick = new TickEnumerator();

        private void Update()
        {
            if (updating)
            {
                // 処理が中断されたとき例外が発生したとみなして、静的フィールドを初期化する。
                Debug.Log($"{nameof(StaticID.Next)}");
                StaticID.Next();
            }
            updating = true;
            tick.Update(_stop ? 0 : 10);
            updating = false;
        }

        public void UpdateOnce()
        {
#if UNITY_EDITOR
            tick.Update(1);
#endif
        }

        public void Reset()
        {
            StaticID.Next();
            tick = new TickEnumerator();
        }
    }
}
