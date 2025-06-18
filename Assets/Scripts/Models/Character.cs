using System.Collections;
using System.Collections.Generic;
using JianAdventure.Managers;
using UnityEngine;
using ProjectJian.SOs;

namespace ProjectJian.Models
{
    public class Character
    {
        public CharacterSO Data { get; private set; }
        public int Health { get; private set; }
        public float Money { get; private set; }
        public int Energy { get; private set; }
        private int diseaseValue;
        public int DiseaseValue
        {
            get => diseaseValue;
            private set => diseaseValue = Mathf.Clamp(value, 1, 100);
        }
        public int CarryCapacity => Data.Strength * 10 + 10;

        public int Frontend => Data.FrontendSkill;
        public int Backend => Data.BackendSkill;
        public int Engine => Data.EngineSkill;
        public int Art => Data.ArtSkill;
        public int Design => Data.DesignSkill;

        public Character(CharacterSO so)
        {
            Data = so;
            Health = so.Health;
            Money = so.StartMoney;
            Energy = so.Energy;
            DiseaseValue = 100;
        }

        public void ModifyHealth(int delta)
        {
            Health = Mathf.Clamp(Health + delta, 0, 100);
            if (Health == 0) GameManager.Instance.GameOver("Health depleted");
        }

        public void ModifyEnergy(int delta)
        {
            Energy = Mathf.Clamp(Energy + delta, 0, 100);
            if (Energy < 30) ModifyHealth(-5);
        }

        public void ModifyMoney(float delta)
        {
            Money += delta;
            if (Money < 0) GameManager.Instance.GameOver("Debt unpaid");
        }

        public void ModifyDisease(int delta)
        {
            DiseaseValue = Mathf.Clamp(DiseaseValue + delta, 1, 100);
        }
    }
}
