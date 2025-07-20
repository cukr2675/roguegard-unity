namespace OchalikeSprites
{
    // 命名メモ: OchalikeSprite や OchalikeMorph ほどの独自性がないうえ、振付けの杖などで表出頻度が高いので OchalikeMotion ではなく SpriteMotion

    /// <summary>
    /// キャラクターのモーション（<see cref="SpritePose"/> を使ったアニメーション）のインターフェース。
    /// 待機アニメーションにも使用するため、状態の種類として扱える値（インスタンス）にする必要がある。
    /// </summary>
    public interface ISpriteMotion
    {
        void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);
    }
}
