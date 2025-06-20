using System.Collections;
using JianAdventure.Managers;
using JianAdventure.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JianAdventure.Managers
{
    /// <summary>
    /// 全局 UI 管理器，负责显示消息、对话、和其他界面元素
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Message Panel")]
        [Tooltip("用于临时提示的面板")]
        public CanvasGroup messagePanel;
        [Tooltip("消息文本组件")]
        public TextMeshProUGUI messageText;
        [Tooltip("提示显示时长（秒）")]
        public float messageDuration = 2f;

        [Header("Dialogue UI")]        
        [Tooltip("对话框根对象")]
        public GameObject dialoguePanel;
        [Tooltip("对话者立绘展示")]
        public Image dialoguePortrait;
        [Tooltip("对话文本展示")]
        public TextMeshProUGUI dialogueContent;
        [Tooltip("选项按钮模板（Prefab 应绑定 Button）")]
        public Button optionButtonPrefab;
        [Tooltip("选项按钮父容器，用于动态生成按钮")]  
        public Transform optionsContainer;

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
        /// 显示 DialogNode 运行时模型对应的对话界面
        /// </summary>
        public void ShowDialogue(DialogueNode node)
        {
            dialoguePanel.SetActive(true);
            dialoguePortrait.sprite = node.Portrait;
            dialogueContent.text = node.Text;

            // 清空旧选项
            foreach (Transform child in optionsContainer)
                Destroy(child.gameObject);

            // 动态生成选项按钮
            foreach (var opt in node.Options)
            {
                var btn = Instantiate(optionButtonPrefab, optionsContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.OptionText;
                btn.onClick.AddListener(() => OnOptionSelected(opt));
            }
        }

        /// <summary>
        /// 隐藏对话界面
        /// </summary>
        public void HideDialogue()
        {
            dialoguePanel.SetActive(false);
        }

        /// <summary>
        /// 选项点击回调，交由 DialogueManager 处理后果和跳转
        /// </summary>
        private void OnOptionSelected(DialogueOption opt)
        {
            HideDialogue();
            DialogueManager.Instance.OnOptionSelected(opt);
        }
        #endregion

        #region Inventory UI (待实现)
        // TODO: 添加背包界面 Show/Hide 以及拖拽逻辑
        #endregion
    }
}
