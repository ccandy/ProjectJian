using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Managers
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [Header("Locations Configuration")]
        [Tooltip("所有地点的 ScriptableObject 列表")]
        public List<LocationSO> Locations;

        [Header("Current State")]
        [Tooltip("当前所在地点")] public LocationSO CurrentLocation;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private void Start()
        {
            // 订阅时间变化事件，刷新可用地点与 NPC
            TimeSystem.Instance.OnTimeChanged += RefreshAvailability;
            RefreshAvailability();
        }

        private void OnDestroy()
        {
            if (TimeSystem.Instance != null)
                TimeSystem.Instance.OnTimeChanged -= RefreshAvailability;
        }

        /// <summary>
        /// 刷新所有地点的开放状态，UI 可监听此方法
        /// </summary>
        public void RefreshAvailability()
        {
            foreach (var loc in Locations)
            {
                bool isOpen = loc.OpenHours.IsOpen();
                // 触发 UI 更新或发送消息，例如：LocationUIController.SetOpen(loc, isOpen)
            }
        }

        /// <summary>
        /// 尝试移动至目标地点
        /// </summary>
        public void TravelTo(LocationSO target)
        {
            if (!target.OpenHours.IsOpen())
            {
                UIManager.Instance.ShowMessage($"地点 {target.LocationName} 暂未开放");
                return;
            }
            if (CurrentLocation == null)
            {
                CurrentLocation = target;
                return;
            }
            float distance = Vector2.Distance(CurrentLocation.Coordinates, target.Coordinates);
            int energyCost = Mathf.CeilToInt(distance);
            GameManager.Instance.Player.ModifyEnergy(-energyCost);

            CurrentLocation = target;

            // 移动后触发事件与刷新 NPC 列表
            EventManager.Instance.CheckEvents(GameManager.Instance.Player);
            RefreshNPCsAtLocation();
        }

        /// <summary>
        /// 刷新当前地点可见的 NPC 列表
        /// </summary>
        private void RefreshNPCsAtLocation()
        {
            var presentNPCs = new List<NPC>();
            foreach (var npc in NPCManager.Instance.NPCs)
            {
                bool atLocation = npc.Data.HomeLocation == CurrentLocation;
                bool inTime = npc.Data.AvailableHours.IsOpen();
                if (atLocation && inTime)
                    presentNPCs.Add(npc);
            }
            // 通知 UI 或其他系统更新 NPC 列表，如：NPCUIController.UpdateNPCs(presentNPCs)
        }
    }
}
