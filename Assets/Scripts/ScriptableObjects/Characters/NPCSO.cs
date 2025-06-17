using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectJian.SOs
{
    //定义：NPC (继承自路人，可增加好感度)
    [CreateAssetMenu(menuName = "JianAdventure/NPCSO", fileName = "NPCSO")]
    public class NPCSO : BystanderSO
    {
        [Header("NPC Affection")]
        [Range(1, 100)] public int Affection;
    }
}



