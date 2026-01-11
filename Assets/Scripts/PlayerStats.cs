using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [Range(0, 100)] public int energy = 60;   // 精力
    [Range(0, 100)] public int health = 60;   // 健康
    [Range(0, 100)] public int family = 60;   // 家庭关系
    public int money = 0;                     // 金钱（可负）

    public int day = 1;                       // 天数计数
}

[System.Serializable]
public class StatDelta
{
    public int energyDelta;
    public int healthDelta;
    public int familyDelta;
    public int moneyDelta;

    public StatDelta(int e, int h, int f, int m)
    {
        energyDelta = e;
        healthDelta = h;
        familyDelta = f;
        moneyDelta = m;
    }
}
