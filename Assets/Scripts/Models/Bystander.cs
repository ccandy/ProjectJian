using System.Collections;
using System.Collections.Generic;
using JianAdventure.SOs;
using UnityEngine;

namespace JianAdventure.Models
{
    public class Bystander
    {
        public BystanderSO Data { get; private set; }
        public Bystander(BystanderSO so)
        {
            Data = so;
        }
    }
}
    
