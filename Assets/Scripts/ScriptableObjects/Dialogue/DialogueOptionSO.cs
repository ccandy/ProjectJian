using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JianAdventure/DialogueOptionSO", fileName = "DialogueOptionSO")]
public class DialogueOptionSO : ScriptableObject
{
    [TextArea] public string OptionText;
    public string NextNodeID;
    // 可在 Inspector 绑定效果脚本或配置字段
}
