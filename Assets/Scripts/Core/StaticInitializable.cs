namespace Roguegard
{
    public class StaticInitializable<T>
    {
        private StaticId staticId;

        private readonly Initializer initializer;

        private T _value;

        public T Value
        {
            get
            {
                if (staticId.IsValid) return _value;

                Value = initializer();
                return _value;
            }
            set
            {
                _value = value;
                staticId = StaticId.Current;
            }
        }

        public delegate T Initializer();

        public StaticInitializable(Initializer initializer)
        {
            this.initializer = initializer;
            _value = default;
            staticId = default;
        }
    }
}
