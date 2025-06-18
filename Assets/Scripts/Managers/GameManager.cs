using ProjectJian.Models;
using ProjectJian.SOs;
using UnityEngine;

namespace JianAdventure.Managers
{
    /// <summary>
    /// 全局游戏管理器，负责玩家实例化及游戏流程控制
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player Setup")]
        [Tooltip("通过 Inspector 指定主角的 ScriptableObject 数据")] 
        public CharacterSO PlayerSO;

        public Character Player { get; private set; }

        private void Awake()
        {
            // 单例检查
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // 创建主角运行时数据
            if (PlayerSO != null)
            {
                Player = new Character(PlayerSO);
            }
            else
            {
                Debug.LogError("GameManager: PlayerSO 未绑定，请在 Inspector 中设置");
            }
        }

        /// <summary>
        /// 结束游戏时调用，传入结束原因
        /// </summary>
        public void GameOver(string reason)
        {
            Debug.LogError($"游戏结束：{reason}");
            // TODO: 跳转到结束界面或播放动画
            // SceneManager.LoadScene("GameOverScene");
        }
    }
}