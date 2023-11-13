using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace RoguegardUnity
{
    public abstract class RogueSaveStream : Stream
    {
        public string Path { get; }
        public RogueSaveStreamStatus Status { get; protected set; }

        protected RogueSaveStream(string path)
        {
            Path = path;
        }

        public virtual void Save(System.Action callback)
        {
            Status = RogueSaveStreamStatus.Succeeded;
            callback();
        }

        protected override void Dispose(bool disposing)
        {
            if (Status == RogueSaveStreamStatus.None)
            {
                Debug.LogError($"{nameof(RogueSaveStream)}: {Path} は {nameof(Save)} の実行前に閉じられました。");
            }

            base.Dispose(disposing);
        }
    }
}
