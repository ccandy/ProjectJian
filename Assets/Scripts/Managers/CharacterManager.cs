using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;


namespace JianAdventure.Managers
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        [Header("Player SO")]
        public CharacterSO PlayerSO;

        public Character Player { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitPlayer();
            }
            else Destroy(gameObject);
        }

        private void InitPlayer()
        {
            if (PlayerSO != null)
                Player = new Character(PlayerSO);
            else
                Debug.LogError("CharacterManager: PlayerSO 未设置");
        }
    }
}
    