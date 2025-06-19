using System;
using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JianAdventure.Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }
        private readonly Dictionary<string, GameEvent> events = new Dictionary<string, GameEvent>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadEvents();
            }
            else Destroy(gameObject);
        }

        private void LoadEvents()
        {
            var eventSOs = Resources.LoadAll<EventSO>("Events");
            foreach (var so in eventSOs)
            {
                var ge = new GameEvent
                {
                    ID = so.EventID,
                    Description = so.Description,
                    Condition = CreateCondition(so.TriggerType, so.Threshold),
                    Action = CreateAction(so.EventID),
                    OneTime = so.OneTime,
                    HasTriggered = false
                };
                events[ge.ID] = ge;
            }
        }

        /// <summary>
        /// 检查并执行所有符合条件的事件
        /// </summary>
        public void CheckEvents(Character player)
        {
            foreach (var ge in events.Values)
            {
                if (ge.HasTriggered && ge.OneTime) continue;
                if (ge.Condition(player))
                {
                    ge.Action(player);
                    if (ge.OneTime) ge.HasTriggered = true;
                }
            }
        }

        /// <summary>
        /// 根据触发类型和阈值创建条件函数
        /// </summary>
        private Func<Character, bool> CreateCondition(JJEventTriggerType type, int threshold)
        {
            switch (type)
            {
                case JJEventTriggerType.EnergyBelow:
                    return (player) => player.Energy < threshold;
                case JJEventTriggerType.MoneyBelow:
                    return (player) => player.Money < threshold;
                case JJEventTriggerType.HealthBelow:
                    return (player) => player.Health < threshold;
                case JJEventTriggerType.DiseaseAbove:
                    return (player) => player.DiseaseValue > threshold;
                default:
                    return (player) => false;
            }
        }

        /// <summary>
        /// 创建对应事件ID的执行逻辑
        /// </summary>
        private Action<Character> CreateAction(string eventID)
        {
            // 可根据 ID 做不同处理，或通过事件中心分发
            return (player) =>
            {
                Debug.Log($"Triggered Event: {eventID}");

                switch (eventID)
                {
                    case "LowEnergy":
                        UIManager.Instance.ShowMessage("警告：精力过低，请休息！");
                        break;
                    case "DebtDue":
                        GameManager.Instance.GameOver("债务到期，游戏结束");
                        break;
                    default:
                        // 默认提示
                        UIManager.Instance.ShowMessage($"事件发生：{eventID}");
                        break;
                }
            };
        }
        
        /// <summary>
        /// 触发指定 ID 的事件（可由 UI 或其他系统主动调用）
        /// </summary>
        public void TriggerEvent(string eventID, Character player)
        {
            if (events.TryGetValue(eventID, out var ge))
            {
                if (!(ge.HasTriggered && ge.OneTime))
                {
                    ge.Action(player);
                    if (ge.OneTime) ge.HasTriggered = true;
                }
            }
            else
            {
                Debug.LogWarning($"EventManager: 未找到事件ID {eventID}");
            }
        }
    }
}
    