using System.Collections;
using System.Collections.Generic;
using JianAdventure.Managers;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Models
{
    /// <summary>
    /// 运行时地点模型，从 LocationSO 构建
    /// </summary>
    public class Location
    {
        public LocationSO Data { get; private set; }
        public string LocationName => Data.LocationName;
        public Vector2 Coordinates => Data.Coordinates;

        public Location(LocationSO so)
        {
            Data = so;
        }

        /// <summary>
        /// 检查此地点当前是否开放
        /// </summary>
        public bool IsOpen()
        {
            return Data.OpenHours.IsOpen();
        }

        /// <summary>
        /// 获取当前此地点可见的 NPC 列表
        /// </summary>
        public List<NPC> GetAvailableNPCs()
        {
            var list = new List<NPC>();
            foreach (var npc in NPCManager.Instance.NPCs)
            {
                if (npc.Data.HomeLocation == Data && npc.Data.AvailableHours.IsOpen())
                {
                    list.Add(npc);
                }
            }
            return list;
        }
    }
}
    
