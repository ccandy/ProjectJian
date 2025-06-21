using JianAdventure.Models;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Managers
{
    /// <summary>
    /// 全局游戏管理器，负责玩家实例化及游戏流程控制
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// 直接从 CharacterManager 获取玩家运行时数据
        /// </summary>
        public Character Player => CharacterManager.Instance.Player;

        private void Awake()
        {
            // 单例模式初始化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 游戏结束处理，传入失败原因
        /// </summary>
        public void GameOver(string reason)
        {
            Debug.LogError($"Game Over: {reason}");
            // TODO: 加载结束场景或弹出失败面板
        }
    }
}