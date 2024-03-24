using System;
using System.Collections;
using System.Collections.Generic;

namespace Objforming
{
    public interface IRelationalComponent
    {
        Type InstanceType { get; }

        /// <summary>
        /// 同じ要素が複数回出現する可能性がある。（同じ型の変数が複数存在するときなど）
        /// </summary>
        IReadOnlyList<Type> FieldTypes { get; }

        /// <summary>
        /// true を返すとき、指定の <see cref="IRelationalComponent"/> を上書きして登録される。
        /// 旧バージョンの <see cref="IRelationalComponent"/> を新バージョンに置き換える際に使用する。
        /// </summary>
        bool Overrides(IRelationalComponent other);
    }
}
