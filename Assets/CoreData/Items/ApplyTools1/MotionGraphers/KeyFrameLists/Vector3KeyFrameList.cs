using System.Diagnostics.CodeAnalysis;

namespace Roguegard
{
    [Objforming.Formable]
    public class Vector3KeyFrameList
    {
        public FloatKeyFrameList XKeys { get; }
        public FloatKeyFrameList YKeys { get; }
        public FloatKeyFrameList ZKeys { get; }

        public Vector3KeyFrameList()
        {
            XKeys = new FloatKeyFrameList();
            YKeys = new FloatKeyFrameList();
            ZKeys = new FloatKeyFrameList();
        }

        [Objforming.CreateInstance, SuppressMessage("Style", "IDE0051")]
        private Vector3KeyFrameList(bool _) { }
    }
}
