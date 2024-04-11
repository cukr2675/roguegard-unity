using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class PostboxInfo
    {
        private readonly List<RoguePost> _posts;

        public Spanning<RoguePost> Posts => _posts;

        private PostboxInfo()
        {
            _posts = new List<RoguePost>();
        }

        public void AddPost(RoguePost post)
        {
            if (post == null) throw new System.ArgumentNullException(nameof(post));

            _posts.Add(post);
        }

        public static PostboxInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            return null;
        }

        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out _))
            {
                var info = new Info();
                info.info = new PostboxInfo();
                obj.SetInfo(info);
            }
            else
            {
                throw new RogueException("上書き不可");
            }
        }

        public static bool RemoveFrom(RogueObj obj)
        {
            return obj.RemoveInfo(typeof(Info));
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public PostboxInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return other is Info otherInfo && info.Posts.Count == 0 && otherInfo.info.Posts.Count == 0;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return null;
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
