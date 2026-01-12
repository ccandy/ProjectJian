using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jianjian/Game Event", fileName = "GE_NewEvent")]
public class GameEventSO : ScriptableObject
{
    public string id;

    [Header("展示文本（随抑郁度变化）")]
    public MoodText description;

    [Header("事件出现条件（不想用就留默认）")]
    public int minDay = 1;
    public int maxDay = 999999;

    public int minMoney = -999999;
    public int maxMoney = 999999;

    public int minDepression = 0;
    public int maxDepression = 100;

    [Header("随机权重（越大越常出现）")]
    [Min(1)] public int weight = 10;

    [Header("选项")]
    public List<ChoiceData> choices = new List<ChoiceData>();

    [System.Serializable]
    public class ChoiceData
    {
        public string label;
        public MoodText resultText;
        public StatDelta delta = new StatDelta(0, 0, 0, 0, 0);
    }

    public bool IsAvailable(PlayerStats s)
    {
        if (s.day < minDay || s.day > maxDay) return false;
        if (s.money < minMoney || s.money > maxMoney) return false;
        if (s.depression < minDepression || s.depression > maxDepression) return false;
        return true;
    }
}
