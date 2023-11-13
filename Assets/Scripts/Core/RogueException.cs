using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueException : System.Exception
    {
        public RogueException() { }

        public RogueException(string message)
            : base(message)
        {
        }

        public RogueException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
