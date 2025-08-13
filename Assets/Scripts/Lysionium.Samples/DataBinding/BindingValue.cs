namespace Lysionium.Samples
{
    public class BindingValue
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnChanged?.Invoke();
            }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnChanged?.Invoke();
            }
        }

        public delegate void OnChangedHandler();

        public event OnChangedHandler OnChanged;

        public BindingValue(string name)
        {
            _name = name;
            _age = 0;
        }

        public override string ToString() => $"{_name} [{_age}]";
    }
}
