using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IMessageWorkListener
    {
        bool CanHandle(RogueObj location, Vector2Int position);

        void Handle(IKeyword keyword, int integer);
        void Handle(IKeyword keyword, float number);
        void Handle(IKeyword keyword, object other);
        void HandleWork(IKeyword keyword, in RogueCharacterWork work);
    }
}
