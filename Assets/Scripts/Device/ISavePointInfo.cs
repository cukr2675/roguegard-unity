using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface ISavePointInfo
    {
        IApplyRogueMethod BeforeSave { get; }

        /// <summary>
        /// <see cref="BeforeSave"/> の直後に実行される可能性があるため、 <see cref="BeforeSave"/> と同じ <see cref="IApplyRogueMethod"/> にする
        /// </summary>
        IApplyRogueMethod AfterLoad { get; }
    }
}
