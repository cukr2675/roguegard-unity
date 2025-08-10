namespace Objforming
{
    public enum FormerMode
    {
        Default,

        /// <summary>
        /// インスタンスのメンバ単体でシリアル化する。
        /// 単一のシリアル化対象メンバを持ち ...List や ...Map のようなラッパーなのがわかりやすい名前のシール型への付与を推奨。
        /// </summary>
        Wrapper,
    }
}
