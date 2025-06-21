using System.Collections;
using System.Collections.Generic;
using JianAdventure.Managers;
using UnityEngine;

namespace JianAdventure.SOs
{
    [System.Serializable]
    public struct TimeWindow
    {
        public int startHour, startMinute;
        public int endHour, endMinute;

        public bool IsOpen() =>
            TimeSystem.Instance.IsWithinTime(startHour, startMinute, endHour, endMinute);
    }

    [CreateAssetMenu(menuName = "JianAdventure/LocationSO", fileName = "LocationSO")]
    public class LocationSO : ScriptableObject
    {
        [Header("基础信息")]
        public string LocationName;
        public Sprite Icon;                      // 地点图标，用于按钮展示
        public Vector2 Coordinates;

        [Header("开放时间")]
        public TimeWindow OpenHours;             // 地点在此时间段开放

        [Header("可见 NPC")]
        public List<NPCSO> AvailableNPCs;        // 在此地点、此时间可能出现的 NPC 列表
    }
}
    