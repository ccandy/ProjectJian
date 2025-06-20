using System.Collections;
using System.Collections.Generic;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Models
{
    public class NPC
    {
        public NPCSO Data { get; private set; }
        public int Affection { get; private set; }

        public NPC(NPCSO so)
        {
            Data = so;
            Affection = so.Affection;
        }

        public void ModifyAffection(int delta)
        {
            Affection = Mathf.Clamp(Affection + delta, 1, 100);
        }
    }
}
    
