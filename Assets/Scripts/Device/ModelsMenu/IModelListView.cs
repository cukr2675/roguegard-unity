using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// モデルリスト用ビュー。
    /// 設定された <see cref="IModelListPresenter"/> とモデルリストをもとに UI を表示するインターフェース。
    /// </summary>
    public interface IModelListView
    {
        void OpenView<T>(
            IModelListPresenter presenter, Spanning<T> modelList, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetPosition();

        void SetPosition(float position);
    }
}
