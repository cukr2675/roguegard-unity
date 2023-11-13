using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    public class BoneNodeBuilder : IBoneNode
    {
        public IBone Bone { get; set; }

        private readonly BoneNodeBuilder[] _children;
        public Spanning<BoneNodeBuilder> Children => _children;
        Spanning<IBoneNode> IBoneNode.Children => _children;

        public BoneNodeBuilder(IBone bone, IEnumerable<BoneNodeBuilder> children)
        {
            Bone = bone;
            _children = children.ToArray();
        }
    }
}
