using System.Collections;
using System.Collections.Generic;
using ProjectJian.SOs;
using UnityEngine;

namespace JianAdventure.Models
{
    public class DialogueNode
    {
        public string NodeID { get; private set; }
        public Sprite Portrait { get; private set; }
        public string Text { get; private set; }
        public List<DialogueOption> Options { get; private set; }
        
        public DialogueNode(DialogueNodeSO so)
        {
            NodeID = so.NodeID;
            Portrait = so.Portrait;
            Text = so.Text;
            Options = new List<DialogueOption>();
            if (so.Options != null)
            {
                foreach (var optSO in so.Options)
                    Options.Add(new DialogueOption(optSO));
            }
        }
        
    }
}

