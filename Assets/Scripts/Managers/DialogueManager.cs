using System.Collections;
using System.Collections.Generic;
using JianAdventure.Models;
using ProjectJian.SOs;
using UnityEngine;

namespace JianAdventure.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private Dictionary<string, DialogueNode> dialogueNodes;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllDialogueNodes();
            }
            else Destroy(gameObject);
        }

        /// <summary>
        /// 从 Resources/Dialogues 加载所有 DialogueNodeSO 并构建运行时模型
        /// </summary>
        private void LoadAllDialogueNodes()
        {
            dialogueNodes = new Dictionary<string, DialogueNode>();
            var nodeSOs = Resources.LoadAll<DialogueNodeSO>("Dialogues");
            foreach (var so in nodeSOs)
            {
                var node = new DialogueNode(so);
                dialogueNodes[so.NodeID] = node;
            }
        }

        /// <summary>
        /// 启动一个对话节点
        /// </summary>
        public void StartDialogue(string nodeID)
        {
            if (dialogueNodes.TryGetValue(nodeID, out var node))
            {
                // 显示 UI
                UIManager.Instance.ShowDialogue(node);
            }
            else
            {
                Debug.LogWarning($"DialogueManager: 未找到节点ID {nodeID}");
            }
        }

        /// <summary>
        /// 选项被选中
        /// </summary>
        public void OnOptionSelected(DialogueOption opt)
        {
            // 执行后果
            if (!string.IsNullOrEmpty(opt.ConsequenceID))
                EventManager.Instance.TriggerEvent(opt.ConsequenceID, GameManager.Instance.Player);

            // 跳转下一节点或结束
            if (!string.IsNullOrEmpty(opt.NextNodeID))
                StartDialogue(opt.NextNodeID);
        }
    }
}

    
