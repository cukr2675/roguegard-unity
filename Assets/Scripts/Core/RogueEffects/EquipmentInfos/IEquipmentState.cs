using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IEquipmentState
    {
        Spanning<IKeyword> Parts { get; }

        int GetLength(IKeyword keyword);

        // 装備品を必ずアイテムリストに表示させるため RogueObj にする。
        // （アイテムリストに表示されないと何を装備しているかわからなくなってしまう　装備中のアイテムを一覧表示したい）
        RogueObj GetEquipment(IKeyword keyword, int index);

        /// <summary>
        /// 装備品オブジェクトを装着するときは、このメソッドではなく <see cref="IEquipmentInfo.BeEquipped"/> を使う。
        /// このメソッドは <see cref="IEquipmentInfo.BeEquipped"/> から実行する。
        /// </summary>
        void SetEquipment(IKeyword keyword, int index, RogueObj equipment);

        /// <summary>
        /// 装備品オブジェクトを解除するときは、このメソッドではなく <see cref="IEquipmentInfo.BeUnequipped"/> を使う。
        /// このメソッドは <see cref="IEquipmentInfo.BeUnequipped"/> から実行する。
        /// </summary>
        void RemoveEquipment(IKeyword keyword, int index);
    }
}
