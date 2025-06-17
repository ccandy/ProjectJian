using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//事件 (SO 存配置)
namespace ProjectJian.SOs
{
    [CreateAssetMenu(menuName = "JianAdventure/EventSO", fileName = "EventSO")]
    public class EventSO : ScriptableObject
    {
        public string EventID;
        public string Description;
        // 可扩展字段：依赖条件、概率、冷却
    }
}

