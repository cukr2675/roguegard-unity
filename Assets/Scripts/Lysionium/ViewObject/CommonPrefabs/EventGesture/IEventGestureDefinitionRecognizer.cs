namespace Lysionium.Views
{
    public interface IEventGestureDefinitionRecognizer
    {
        /// <summary>
        /// この値が true のとき、処理中のイベントジェスチャが残っていても、このイベントジェスチャだけ即確定する。
        /// 他の処理中のイベントジェスチャはそのまま判定継続される
        /// (用途: ダブルクリック時とシングルクリック時の処理を両方実行したい場合)
        /// </summary>
        bool IsIndependent { get; }

        float GestureProgress { get; }

        void ResetRecognizer();

        void UpdateRecognizer(EventGestureRecognizer recognizer);
    }
}
