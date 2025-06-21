using System.Collections;
using System.Collections.Generic;
using JianAdventure.SOs;
using UnityEngine;
using UnityEngine.UI;


namespace JianAdventure.Managers
{
    /// <summary>
    /// 地图场景控制器：管理地图 HUD 和地点按钮
    /// </summary>
    public class MapSceneController : MonoBehaviour
    {
        [Header("HUD References")]
        public Text timeText;
        public Text regionText;
        public Text energyText;

        [Header("Location Buttons")]        
        public Button locationButtonPrefab;  // Prefab 包含 Image + Button
        public Transform buttonsContainer;

        private List<Button> locationButtons = new List<Button>();

        private void Start()
        {
            // 初始化 HUD
            UpdateHUD();

            // 订阅时间变化，更新显示
            TimeSystem.Instance.OnTimeChanged += UpdateTime;
            
            // 创建地点按钮
            foreach (var loc in MapManager.Instance.Locations)
            {
                var btn = Instantiate(locationButtonPrefab, buttonsContainer);
                btn.GetComponentInChildren<Text>().text = loc.LocationName;
                btn.image.sprite = loc.Icon; // 假设 LocationSO 增加了 Icon 字段
                btn.onClick.AddListener(() => OnLocationClicked(loc));
                locationButtons.Add(btn);
            }

            // 初始按钮状态
            RefreshLocationButtons();
        }

        private void OnDestroy()
        {
            if (TimeSystem.Instance != null)
                TimeSystem.Instance.OnTimeChanged -= UpdateTime;
        }

        /// <summary>
        /// 点击地点按钮
        /// </summary>
        private void OnLocationClicked(LocationSO loc)
        {
            if (GameManager.Instance.Player.Energy <= 0)
            {
                UIManager.Instance.ShowMessage("精力不足，无法移动！");
                return;
            }

            MapManager.Instance.TravelTo(loc);
            UpdateHUD();
            RefreshLocationButtons();
        }

        /// <summary>
        /// 更新 HUD 文本
        /// </summary>
        private void UpdateHUD()
        {
            // 时间显示 格式：YYYY/MM/DD HH:MM
            var ts = TimeSystem.Instance;
            timeText.text = $"{ts.Year}/{ts.Month:00}/{ts.Day:00} {ts.Hour:00}:{ts.Minute:00}";
            
            // 当前地区名
            var curr = MapManager.Instance.CurrentLocation;
            regionText.text = curr != null ? curr.LocationName : "无";
            
            // 精力
            energyText.text = $"Energy: {GameManager.Instance.Player.Energy}";
        }

        /// <summary>
        /// 响应时间改变，仅更新时间显示
        /// </summary>
        private void UpdateTime()
        {
            UpdateHUD();
            RefreshLocationButtons();
        }

        /// <summary>
        /// 刷新所有地点按钮的可交互状态
        /// </summary>
        private void RefreshLocationButtons()
        {
            foreach (var btn in locationButtons)
            {
                // 获取对应 LocationSO
                var locName = btn.GetComponentInChildren<Text>().text;
                var locSO = MapManager.Instance.Locations.Find(l => l.LocationName == locName);
                bool canMove = GameManager.Instance.Player.Energy > 0 && locSO.OpenHours.IsOpen();
                btn.interactable = canMove;
            }
        }
    }
}
    
