using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;


namespace JianAdventure.Managers
{
    public class NPCManager : MonoBehaviour
    {
        public static NPCManager Instance { get; private set; }

        [Header("NPC SOs")]
        public List<NPCSO> NPCSOList;

        public List<NPC> NPCs { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitNPCs();
            }
            else Destroy(gameObject);
        }

        private void InitNPCs()
        {
            NPCs = new List<NPC>();
            foreach (var so in NPCSOList)
                NPCs.Add(new NPC(so));
        }
    }
}
    
