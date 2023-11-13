using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Credit")]
    public class CreditData : ScriptableObject
    {
        [SerializeField] private string _name = null;
        public string Name => _name;

        [SerializeField, TextArea(3, 60)] private string _details = null;
        public string Details => _details;
    }
}
