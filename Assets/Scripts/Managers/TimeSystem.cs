using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JianAdventure.Managers
{
    public class TimeSystem : MonoBehaviour
    {
        public static TimeSystem Instance { get; private set; }

        [Header("Initial Time")]        
        public int StartYear = 2025;
        [Range(1,12)] public int StartMonth = 1;
        [Range(1,31)] public int StartDay = 1;
        [Range(0,23)] public int StartHour = 8;
        [Range(0,59)] public int StartMinute = 0;

        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }
        public int Hour { get; private set; }
        public int Minute { get; private set; }

        /// <summary>
        /// 当时间变化时触发（每次 Advance 后）
        /// </summary>
        public event Action OnTimeChanged;

        private readonly int[] daysInMonth = new int[] { 31,28,31,30,31,30,31,31,30,31,30,31 };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                ResetTime();
            }
            else Destroy(gameObject);
        }

        /// <summary>
        /// 重置到起始时间
        /// </summary>
        public void ResetTime()
        {
            Year = StartYear;
            Month = StartMonth;
            Day = StartDay;
            Hour = StartHour;
            Minute = StartMinute;
            OnTimeChanged?.Invoke();
        }

        /// <summary>
        /// 推进指定分钟数，并处理日期溢出
        /// </summary>
        public void AdvanceMinutes(int minutes)
        {
            Minute += minutes;
            NormalizeTime();
            OnTimeChanged?.Invoke();
        }

        /// <summary>
        /// 推进指定小时数
        /// </summary>
        public void AdvanceHours(int hours)
        {
            AdvanceMinutes(hours * 60);
        }

        private void NormalizeTime()
        {
            // 分钟->小时
            if (Minute >= 60)
            {
                Hour += Minute / 60;
                Minute %= 60;
            }
            // 小时->天
            if (Hour >= 24)
            {
                Day += Hour / 24;
                Hour %= 24;
            }
            // 天->月
            // 闰年处理简化，不考虑闰年
            int dim = daysInMonth[Month - 1];
            if (Day > dim)
            {
                Day -= dim;
                Month++;
                if (Month > 12)
                {
                    Month = 1;
                    Year++;
                }
            }
        }

        /// <summary>
        /// 检查当前时间是否在开店/出现NPC的时间段
        /// </summary>
        /// <param name="startHour">开始小时（含）</param>
        /// <param name="startMinute">开始分钟（含）</param>
        /// <param name="endHour">结束小时（不含）</param>
        /// <param name="endMinute">结束分钟（不含）</param>
        public bool IsWithinTime(int startHour, int startMinute, int endHour, int endMinute)
        {
            var now = Hour * 60 + Minute;
            var start = startHour * 60 + startMinute;
            var end = endHour * 60 + endMinute;
            if (start <= end)
                return now >= start && now < end;
            else
                return now >= start || now < end; // 跨天逻辑
        }
    }    
}

