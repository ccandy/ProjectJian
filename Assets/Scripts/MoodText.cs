using UnityEngine;

[System.Serializable]
public struct MoodText
{
    [TextArea(2, 6)] public string normal;     // depression < 40
    [TextArea(2, 6)] public string gloomy;     // 40..79
    [TextArea(2, 6)] public string meltdown;   // >=80

    public string Resolve(int depression)
    {
        if (depression >= 80 && !string.IsNullOrEmpty(meltdown)) return meltdown;
        if (depression >= 40 && !string.IsNullOrEmpty(gloomy)) return gloomy;
        return normal ?? "";
    }
}
