using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IRogueDevice
    {
        string Name { get; }

        string Version { get; }

        string Description { get; }

        RogueObj Player { get; }

        bool CalledSynchronizedView { get; }

        bool NextStay { get; }

        void Close();

        void Next();

        void Update();

        void AddInt(IKeyword keyword, int integer);
        void AddFloat(IKeyword keyword, float number);
        void AddWork(IKeyword keyword, in RogueCharacterWork work);
        void AddObject(IKeyword keyword, object obj);

        void AddMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        /// <summary>
        /// 指定の位置が視界範囲内であれば true を取得する。
        /// </summary>
        bool VisibleAt(RogueObj location, Vector2Int position);

        void UpdateCharacters();
    }
}
