using System.Collections;
using JianAdventure.Managers;
using ProjectJian.SOs;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JianAdventure.Managers
{
    /// <summary>
    /// 全局 UI 管理器，负责显示消息、对话框及其他界面元素
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Message Panel")]
        [Tooltip("用于临时提示的面板")] public CanvasGroup messagePanel;
        [Tooltip("消息文本组件")] public TextMeshProUGUI messageText;
        [Tooltip("提示显示时长（秒）")] public float messageDuration = 2f;

        [Header("Dialogue UI")]        
        [Tooltip("对话框根对象")] public GameObject dialoguePanel;
        [Tooltip("对话者立绘展示")] public Image dialoguePortrait;
        [Tooltip("对话文本展示")] public TextMeshProUGUI dialogueContent;
        [Tooltip("选项按钮模板")] public Button optionButtonPrefab;
        [Tooltip("选项按钮父容器")] public Transform optionsContainer;

        private void Awake()
        {
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

        #region Message
        /// <summary>
        /// 弹出提示消息
        /// </summary>
        public void ShowMessage(string message)
        {
            StopAllCoroutines();
            StartCoroutine(DoShowMessage(message));
        }

        private IEnumerator DoShowMessage(string message)
        {
            messageText.text = message;
            messagePanel.alpha = 1;
            messagePanel.blocksRaycasts = true;
            yield return new WaitForSeconds(messageDuration);
            messagePanel.alpha = 0;
            messagePanel.blocksRaycasts = false;
        }
        #endregion

        #region Dialogue
        /// <summary>
        /// 开始对话，显示对话 UI
        /// </summary>
        public void ShowDialogue(DialogueNodeSO node)
        {
            dialoguePanel.SetActive(true);
            dialoguePortrait.sprite = node.Portrait;
            dialogueContent.text = node.Text;

            // 清空旧选项
            foreach (Transform t in optionsContainer) Destroy(t.gameObject);

            // 创建新选项
            foreach (var opt in node.Options)
            {
                var btn = Instantiate(optionButtonPrefab, optionsContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.OptionText;
                btn.onClick.AddListener(() => OnOptionSelected(opt));
            }
        }

        /// <summary>
        /// 处理选项点击
        /// </summary>
        private void OnOptionSelected(DialogueOptionSO opt)
        {
            // 隐藏对话框
            dialoguePanel.SetActive(false);
            // 执行选项对应逻辑，通过配置的 ConsequenceID 触发事件
            if (!string.IsNullOrEmpty(opt.ConsequenceID))
            {
                EventManager.Instance.TriggerEvent(opt.ConsequenceID, GameManager.Instance.Player);
            }
            // 跳转下一对话节点
            if (!string.IsNullOrEmpty(opt.NextNodeID))
            {
                var nextNode = Resources.Load<DialogueNodeSO>("Dialogues/" + opt.NextNodeID);
                if (nextNode != null) ShowDialogue(nextNode);
            }
        }
        #endregion

        #region Inventory UI (示例)
        // TODO: 添加显示/隐藏背包界面的接口
        #endregion
    }
}
