using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    // Lysionium.HandlerRules のように Observer 風に GetName や HandleClick に分岐させればハンドラのダウンキャストを無くせるが
    // ・ViewElement が利用するハンドラをインターフェースとして保持できないため、依存関係が不明瞭になる
    // ・ハンドラの実装と呼び出しが直感的ではなくなる（参照やスタックトレースが追いづらくなる）
    // などデメリットがある

    /// <summary>
    /// <see cref="IElementsSubview"/> に各要素をどのように扱わせるかを設定するインターフェース。
    /// 一つのメニュー画面が複数の <see cref="IElementHandler"/> を持つ可能性があるため分けて考える。
    /// </summary>
    public interface IElementHandler
    {
        string GetName(object element, IListMenuManager manager, IListMenuArg arg);

        // 右クリックなど種類が増えるとメソッド一つでは足りない
        // メソッドが増えると実装が面倒
        // 戻るボタンは常に esc キーをバインドするのであれば Style と統合したほうがスムーズ
        //InputAction GetKeyBind(object element, IListMenuManager manager, IListMenuArg arg);

        // Roguegard の実装を見ると、アイコン以外の情報も同時に返すほうが効率的なため使用していない
        // 使用するとしてもキーバインドと同じように Style でキーワードを渡すべき？
        // GetName はデバッグにも使用できるが GetIcon は不向き
        //Sprite GetIcon(object element, IListMenuManager manager, IListMenuArg arg);

        // Name と Style はラベルをはじめとしたほとんどのコントロールで使うため実装する
        string GetStyle(object element, IListMenuManager manager, IListMenuArg arg);
    }
}
