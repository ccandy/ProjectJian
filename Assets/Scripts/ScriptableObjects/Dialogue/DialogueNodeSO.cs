using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//对话节点
[CreateAssetMenu(menuName = "JianAdventure/DialogueNodeSO", fileName = "DialogueNodeSO")]
public class DialogueNodeSO : ScriptableObject
{
    public string NodeID;
    public Sprite Portrait;
    [TextArea] public string Text;
    public List<DialogueOptionSO> Options;
}
