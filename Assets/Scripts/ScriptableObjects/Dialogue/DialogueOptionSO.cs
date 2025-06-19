using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectJian.SOs
{
    [CreateAssetMenu(menuName = "JianAdventure/DialogueOptionSO", fileName = "DialogueOptionSO")]
    public class DialogueOptionSO : ScriptableObject
    {
        [TextArea]
        public string OptionText;

        [Tooltip("对话结束后要跳转到的下一个节点 ID")]
        public string NextNodeID;

        [Header("Option Effects")]
        [Tooltip("在 Inspector 中指定选项触发的额外事件 ID 或方法")]
        public string ConsequenceID;

    }
}
