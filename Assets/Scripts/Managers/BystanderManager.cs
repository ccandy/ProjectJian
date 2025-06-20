using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;


namespace JianAdventure.Managers
{
    public class BystanderManager : MonoBehaviour
    {
        public static BystanderManager Instance { get; private set; }

        [Header("Bystander SOs")]
        public List<BystanderSO> BystanderSOList;

        public List<Bystander> Bystanders { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitBystanders();
            }
            else Destroy(gameObject);
        }

        private void InitBystanders()
        {
            Bystanders = new List<Bystander>();
            foreach (var so in BystanderSOList)
                Bystanders.Add(new Bystander(so));
        }
    }

}
