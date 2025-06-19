using System.Collections;
using System.Collections.Generic;
using JianAdventure.Managers;
using JianAdventure.SOs;
using UnityEngine;


namespace JianAdventure.Models
{
    public class Backpack
    {
        // 背包容量（体积）上限
        public int Capacity { get; private set; } = 20;

        // 当前存放的道具实例
        private readonly List<ItemInstance> items = new List<ItemInstance>();

        /// <summary>
        /// 获取已用容量（物品体积总和）
        /// </summary>
        public int CurrentVolume
        {
            get
            {
                int volume = 0;
                foreach (var inst in items)
                    volume += inst.Data.Volume;
                return volume;
            }
        }

        /// <summary>
        /// 获取已用重量（物品重量总和）
        /// </summary>
        public int CurrentWeight
        {
            get
            {
                int weight = 0;
                foreach (var inst in items)
                    weight += inst.Data.Weight;
                return weight;
            }
        }

        /// <summary>
        /// 当前背包中的物品列表，只读
        /// </summary>
        public IReadOnlyList<ItemInstance> Items => items.AsReadOnly();

        /// <summary>
        /// 尝试添加物品到背包
        /// </summary>
        /// <param name="so">要添加的道具SO</param>
        /// <param name="owner">主角，用于检查负重</param>
        /// <returns>添加成功返回 true，否则 false</returns>
        public bool AddItem(ItemSO so, Character owner)
        {
            // 检查体积
            if (CurrentVolume + so.Volume > Capacity)
                return false;
            
            // 检查负重
            if (CurrentWeight + so.Weight > owner.CarryCapacity)
                return false;

            // 添加
            items.Add(new ItemInstance(so));
            return true;
        }

        /// <summary>
        /// 从背包移除指定实例
        /// </summary>
        public void RemoveItem(ItemInstance instance)
        {
            if (items.Contains(instance))
                items.Remove(instance);
        }

        /// <summary>
        /// 使用道具，调用拥有者效果并释放背包空间
        /// </summary>
        public bool UseItem(ItemInstance instance)
        {
            if (!items.Contains(instance)) return false;
            
            // 对主角生效
            GameManager.Instance.Player.ModifyHealth(instance.Data.HealthEffect);

            // 移除道具
            items.Remove(instance);
            return true;
        }

        /// <summary>
        /// 升级背包容量
        /// </summary>
        /// <param name="newCapacity">新容量上限</param>
        public void UpgradeCapacity(int newCapacity)
        {
            Capacity = newCapacity;
        }
    }
}
    
