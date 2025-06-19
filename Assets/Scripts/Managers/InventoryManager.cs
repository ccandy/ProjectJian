using System.Collections;
using System.Collections.Generic;
using JianAdventure.Managers;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Managers
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        // 持有当前角色的背包
        public Backpack PlayerBackpack { get; private set; }

        private void Awake()
        {
            // 单例机制
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                PlayerBackpack = new Backpack();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 添加道具
        /// </summary>
        public bool TryAddItem(ItemSO so)
        {
            return PlayerBackpack.AddItem(so, GameManager.Instance.Player);
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        public bool TryUseItem(ItemInstance instance)
        {
            return PlayerBackpack.UseItem(instance);
        }

        /// <summary>
        /// 移除道具
        /// </summary>
        public void RemoveItem(ItemInstance instance)
        {
            PlayerBackpack.RemoveItem(instance);
        }
    }
}
    