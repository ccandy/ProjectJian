using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectJian.SOs
{
    [CreateAssetMenu(menuName = "JianAdventure/ItemSO", fileName = "ItemSO")]
    public class ItemSO : ScriptableObject
    {
        public Sprite Icon;
        public string ItemName;
        [TextArea] public string Description;
        public int HealthEffect;
        public int Volume;
        public int Weight;
    }
}

