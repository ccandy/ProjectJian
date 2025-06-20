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

        public bool IsOpen() => TimeSystem.Instance.IsWithinTime(startHour, startMinute, endHour, endMinute);
    }
    
    [CreateAssetMenu(menuName = "JianAdventure/LocationSO", fileName = "LocationSO")]
    public class LocationSO : ScriptableObject
    {
        public string LocationName;
        public Vector2 Coordinates;
        public TimeWindow OpenHours;      // 地点开放时间段
        public List<NPCSO> AvailableNPCs; // 在此地点可能出现的 NPC
    }
}
    