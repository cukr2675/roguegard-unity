using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// モデルリスト用プレゼンタ。
    /// <see cref="IModelListView"/> にモデルリストの要素をどのように扱わせるかを設定するインターフェース。
    /// 一つの <see cref="IModelsMenu"/> が複数の <see cref="IModelListPresenter"/> を持つ可能性があるため分けて考える。
    /// </summary>
    public interface IModelListPresenter
    {
        string GetItemName(object modelListItem, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void ActivateItem(object modelListItem, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
