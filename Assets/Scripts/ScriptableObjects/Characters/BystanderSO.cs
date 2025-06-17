using System;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectJian.SOs
{
    //路人NPC
    [CreateAssetMenu(menuName = "ProjectJian/BystanderSO", fileName = "BystanderSO")]
    public class BystanderSO : ScriptableObject
    {
        public string Name;
        public Sprite Portrait;
    }
}

