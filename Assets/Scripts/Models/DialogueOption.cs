using System.Collections;
using System.Collections.Generic;
using ProjectJian.SOs;
using UnityEngine;

namespace JianAdventure.Models
{
    public class DialogueOption
    {
        public string OptionText { get; private set; }
        public string NextNodeID { get; private set; }
        public string ConsequenceID { get; private set; }
        
        public DialogueOption(DialogueOptionSO so)
        {
            OptionText = so.OptionText;
            NextNodeID = so.NextNodeID;
            ConsequenceID = so.ConsequenceID;
        }
        
    }
}
    
