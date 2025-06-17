using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectJian.SOs
{
    public class CharacterSO : NPCSO
    {
        [Header("Base Stats")]
        [Range(1, 100)] public int Health;
        public float StartMoney;
        [Range(1, 100)] public int Energy;
        [HideInInspector] [Range(1, 100)] public int DiseaseValue;
        [Range(1, 5)] public int Strength;

        [Header("Skills")]
        [Range(1,100)] public int FrontendSkill;
        [Range(1,100)] public int BackendSkill;
        [Range(1,100)] public int EngineSkill;
        [Range(1,100)] public int ArtSkill;
        [Range(1,100)] public int DesignSkill;
    }

}

