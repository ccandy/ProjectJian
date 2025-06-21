using System.Collections;
using System.Collections.Generic;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Models
{
    /// <summary>
    /// 好感度 NPC 运行时模型
    /// </summary>
    public class NPC
    {
        public NPCSO Data { get; private set; }
        public int Affection { get; private set; }

        public NPC(NPCSO so)
        {
            Data = so;
            Affection = so.Affection;
        }

        /// <summary>
        /// 修改 NPC 好感度
        /// </summary>
        public void ModifyAffection(int delta)
        {
            Affection = Mathf.Clamp(Affection + delta, 1, 100);
        }
    }
}
    
