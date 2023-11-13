using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    internal class WaitTimer
    {
        private float seconds;

        public bool Wait => seconds > 0f;

        public void Reset()
        {
            seconds = 0f;
        }

        public void UpdateTimer(int deltaTime)
        {
            seconds -= deltaTime / 60f;
        }

        public void Start(float seconds)
        {
            this.seconds = seconds;
        }
    }
}
