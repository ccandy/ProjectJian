using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//事件 (SO 存配置)
namespace JianAdventure.SOs
{
    public enum JJEventTriggerType
    {
        None,
        EnergyBelow,
        MoneyBelow,
        HealthBelow,
        DiseaseAbove
    }
    [CreateAssetMenu(menuName = "JianAdventure/EventSO", fileName = "EventSO")]
    public class EventSO : ScriptableObject
    {
        public string EventID;
        public string Description;
        public JJEventTriggerType TriggerType;
        public int Threshold;
        public bool OneTime = true;
    }
}

