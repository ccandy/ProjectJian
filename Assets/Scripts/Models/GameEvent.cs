using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JianAdventure.Models
{
    public class GameEvent
    {
        public string ID;
        public string Description;
        public Func<Character, bool> Condition;
        public Action<Character> Action;
        public bool OneTime;
        public bool HasTriggered;
    }
}
    
