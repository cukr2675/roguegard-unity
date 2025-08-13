using UnityEngine;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "Roguegard Unity/Credit")]
    public class CreditData : ScriptableObject
    {
        [SerializeField] private string _name = null;
        public string Name => _name;

        [SerializeField, TextArea(3, 60)] private string _details = null;
        public string Details => _details;
    }
}
