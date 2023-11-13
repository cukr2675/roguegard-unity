using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class ScriptableLoader : ScriptableObject
    {
        /// <summary>
        /// ロード進捗ゲージ全体での重み。大きいほどゲージの広範囲を占め、ロード完了時のゲージの進みが大きくなる
        /// </summary>
        public virtual float ProgressBarWeight => 1f;

        public abstract IEnumerator LoadAsync();

        public abstract void TestLoad();
    }
}
