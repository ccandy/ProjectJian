using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;
using UnityEngine.UI;


namespace JianAdventure.Managers
{
    /// <summary>
    /// 地图场景控制器：管理地图 HUD、地点按钮与位置指示动画
    /// </summary>
    public class MapSceneController : MonoBehaviour
    {
        [Header("HUD References")]
        public Text timeText;
        public Text regionText;
        public Text energyText;

        [Header("Map UI")]        
        [Tooltip("地图背景 Image (UI)")]
        public Image mapBackground;
        [Tooltip("玩家图标 Image，用于移动动画")]
        public Image playerIcon;
        [Tooltip("地图坐标最小值 (对应地图左下)")]
        public Vector2 mapMinCoord = new Vector2(0, 0);
        [Tooltip("地图坐标最大值 (对应地图右上)")]
        public Vector2 mapMaxCoord = new Vector2(10, 10);
        [Tooltip("玩家图标移动速度 (UI 单位/秒)")]
        public float iconMoveSpeed = 200f;

        [Header("Location Buttons")]        
        public Button locationButtonPrefab;
        public Transform buttonsContainer;

        [Header("NPC Panel")]        
        public Button npcButtonPrefab;
        public Transform npcButtonsContainer;

        private List<Button> locationButtons = new List<Button>();
        private List<Location> runtimeLocations = new List<Location>();

        private void Start()
        {
            UpdateHUD();
            TimeSystem.Instance.OnTimeChanged += OnTimeChanged;

            // 初始化地点与按钮
            foreach (var locSO in MapManager.Instance.Locations)
            {
                var loc = new Location(locSO);
                runtimeLocations.Add(loc);

                var btn = Instantiate(locationButtonPrefab, buttonsContainer);
                btn.GetComponentInChildren<Text>().text = loc.LocationName;
                btn.image.sprite = loc.Data.Icon;
                btn.onClick.AddListener(() => OnLocationClicked(loc));
                locationButtons.Add(btn);
            }
            RefreshLocationButtons();

            // 初始 NPC 刷新
            if (runtimeLocations.Count > 0)
                RefreshNPCButtons(runtimeLocations[0]);

            // 初始玩家图标定位
            var startPos = MapManager.Instance.CurrentLocation != null
                ? MapManager.Instance.CurrentLocation.Coordinates
                : runtimeLocations[0].Data.Coordinates;
            Vector2 uiPos = WorldToUIPosition(startPos);
            playerIcon.rectTransform.anchoredPosition = uiPos;
        }

        private void OnDestroy()
        {
            if (TimeSystem.Instance != null)
                TimeSystem.Instance.OnTimeChanged -= OnTimeChanged;
        }

        private void OnTimeChanged()
        {
            UpdateHUD();
            RefreshLocationButtons();
            var currSO = MapManager.Instance.CurrentLocation;
            var currLoc = currSO != null ? new Location(currSO) : null;
            RefreshNPCButtons(currLoc);
        }

        private void OnLocationClicked(Location loc)
        {
            if (GameManager.Instance.Player.Energy <= 0)
            {
                UIManager.Instance.ShowMessage("精力不足，无法移动！");
                return;
            }
            // 移动逻辑
            MapManager.Instance.TravelTo(loc.Data);
            UpdateHUD();
            RefreshLocationButtons();
            RefreshNPCButtons(loc);

            // 播放图标移动动画
            Vector2 targetUI = WorldToUIPosition(loc.Data.Coordinates);
            StopAllCoroutines();
            StartCoroutine(MovePlayerIcon(targetUI));
        }

        private void UpdateHUD()
        {
            var ts = TimeSystem.Instance;
            timeText.text = $"{ts.Year}/{ts.Month:00}/{ts.Day:00} {ts.Hour:00}:{ts.Minute:00}";
            var currSO = MapManager.Instance.CurrentLocation;
            regionText.text = currSO != null ? currSO.LocationName : "无";
            energyText.text = $"Energy: {GameManager.Instance.Player.Energy}";
        }

        private void RefreshLocationButtons()
        {
            for (int i = 0; i < locationButtons.Count; i++)
            {
                var btn = locationButtons[i];
                var loc = runtimeLocations[i];
                btn.interactable = GameManager.Instance.Player.Energy > 0 && loc.IsOpen();
            }
        }

        /// <summary>
        /// 刷新 NPC 按钮，根据地点和时间决定可见 NPC
        /// </summary>
        private void RefreshNPCButtons(Location loc)
        {
            foreach (Transform child in npcButtonsContainer)
                Destroy(child.gameObject);
            if (loc == null) return;
            var npcs = loc.GetAvailableNPCs();
            foreach (var npc in npcs)
            {
                var btn = Instantiate(npcButtonPrefab, npcButtonsContainer);
                btn.GetComponentInChildren<Text>().text = npc.Data.Name;
                btn.image.sprite = npc.Data.Portrait;
                btn.onClick.AddListener(() => DialogueManager.Instance.StartDialogue(npc.Data.Name));
            }
        }

        /// <summary>
        /// 将世界坐标映射到 UI 坐标
        /// </summary>
        private Vector2 WorldToUIPosition(Vector2 worldPos)
        {
            if (mapBackground == null) return Vector2.zero;
            RectTransform bgRect = mapBackground.rectTransform;
            Vector2 normalized = new Vector2(
                (worldPos.x - mapMinCoord.x) / (mapMaxCoord.x - mapMinCoord.x),
                (worldPos.y - mapMinCoord.y) / (mapMaxCoord.y - mapMinCoord.y)
            );
            Vector2 localPos = new Vector2(
                bgRect.rect.xMin + normalized.x * bgRect.rect.width,
                bgRect.rect.yMin + normalized.y * bgRect.rect.height
            );
            return localPos;
        }

        /// <summary>
        /// 平滑移动玩家图标到目标 UI 位置
        /// </summary>
        private IEnumerator MovePlayerIcon(Vector2 targetPos)
        {
            while (Vector2.Distance(playerIcon.rectTransform.anchoredPosition, targetPos) > 0.1f)
            {
                playerIcon.rectTransform.anchoredPosition = Vector2.MoveTowards(
                    playerIcon.rectTransform.anchoredPosition,
                    targetPos,
                    iconMoveSpeed * Time.deltaTime
                );
                yield return null;
            }
            playerIcon.rectTransform.anchoredPosition = targetPos;
        }
    }
}

    
