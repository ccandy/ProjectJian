using System;
using System.Collections.Generic;
using UnityEngine;


namespace JianAdventure.SOs
{
    //路人NPC
    [CreateAssetMenu(menuName = "JianAdventure/BystanderSO", fileName = "BystanderSO")]
    public class BystanderSO : ScriptableObject
    {
        public string Name;
        public Sprite Portrait;
    }
}

