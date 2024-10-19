using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueDevice
    {
        RogueObj Player { get; }

        RogueObj Subject { get; }

        RogueObj World { get; }

        /// <summary>
        /// この値が true のとき同期アニメーションの待機を要求する
        /// </summary>
        bool HasSynchronizedWork { get; }

        void Close();

        bool UpdateAndGetAllowStepTurn();

        void AfterStepTurn();

        void AddInt(IKeyword keyword, int integer);
        void AddFloat(IKeyword keyword, float number);
        void AddObject(IKeyword keyword, object obj);
        void AddWork(IKeyword keyword, in RogueCharacterWork work);
        void AddMenu(RogueMenuScreen menu, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
